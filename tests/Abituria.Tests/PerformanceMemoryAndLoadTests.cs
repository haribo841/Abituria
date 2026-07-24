using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Avalonia.Headless.XUnit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Sdk;

namespace Abituria.Tests;

/// <summary>
/// Formalne, automatyczne bramki niefunkcjonalne. Progi są celowo szerokie,
/// aby wykrywać regresje rzędu wielkości, a nie różnice między maszynami CI.
/// </summary>
public sealed class PerformanceMemoryAndLoadTests
{
    private const int CalculatorIterations = 10_000;
    private const int ParallelCalculatorOperations = 40_000;
    private const int ContentReloads = 20;
    private const int ProgressRecordCount = 5_000;
    private const int ProgressReadCount = 3;

    [Fact]
    [Trait("Category", "Performance")]
    [Trait("Category", "Memory")]
    public void Calculator_mixed_workload_meets_throughput_and_allocation_budgets()
    {
        var calculator = new ExpressionCalculator();
        var cases = new[]
        {
            new CalculatorCase("((12,5+7,5)*3-4)/2", 28d),
            new CalculatorCase("root(3;125)+sqrt(144)-2^5", -15d),
            new CalculatorCase("2(3+4)-sqrt(81)+root(4;16)", 7d)
        };

        RunCalculatorCases(calculator, cases, 100);
        ForceFullCollection();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();
        RunCalculatorCases(calculator, cases, CalculatorIterations);
        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        var operations = CalculatorIterations * cases.Length;
        var bytesPerOperation = allocatedBytes / (double)operations;

        ReportMetric(
            "calculator-mixed",
            $"operations={operations}; elapsedMs={stopwatch.Elapsed.TotalMilliseconds:F1}; " +
            $"operationsPerSecond={operations / stopwatch.Elapsed.TotalSeconds:F0}; " +
            $"allocatedBytes={allocatedBytes}; bytesPerOperation={bytesPerOperation:F1}");

        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(15),
            $"Kalkulator wykonał {operations} reprezentatywnych obliczeń w {stopwatch.Elapsed}.");
        Assert.True(
            bytesPerOperation <= 12 * 1024d,
            $"Kalkulator zaalokował średnio {bytesPerOperation:F1} B na obliczenie.");
    }

    [Fact]
    [Trait("Category", "Load")]
    public void Calculator_parallel_workload_finishes_without_errors_within_budget()
    {
        var calculator = new ExpressionCalculator();
        var failures = new ConcurrentQueue<string>();
        var workers = Math.Clamp(Environment.ProcessorCount, 1, 8);
        var stopwatch = Stopwatch.StartNew();

        Parallel.For(
            0,
            ParallelCalculatorOperations,
            new ParallelOptions { MaxDegreeOfParallelism = workers },
            index =>
            {
                var left = index % 83 + 1;
                var middle = index % 17 + 1;
                var multiplier = index % 9 + 1;
                var denominator = index % 13 + 1;
                var expression = $"(({left}+{middle})*{multiplier})/{denominator}";
                var expected = ((left + middle) * multiplier) / (double)denominator;

                try
                {
                    var result = calculator.Evaluate(expression);
                    if (!result.Success || result.Value is not double actual || Math.Abs(actual - expected) > 1E-12d)
                        failures.Enqueue($"{expression}: {result.ErrorCode}: {result.Message}: {result.Value}");
                }
                catch (Exception exception)
                {
                    failures.Enqueue($"{expression}: {exception.GetType().Name}: {exception.Message}");
                }
            });

        stopwatch.Stop();
        ReportMetric(
            "calculator-parallel",
            $"operations={ParallelCalculatorOperations}; workers={workers}; " +
            $"elapsedMs={stopwatch.Elapsed.TotalMilliseconds:F1}; " +
            $"operationsPerSecond={ParallelCalculatorOperations / stopwatch.Elapsed.TotalSeconds:F0}; failures={failures.Count}");

        Assert.Empty(failures);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(20),
            $"Równoległe obciążenie kalkulatora trwało {stopwatch.Elapsed}.");
    }

    [AvaloniaFact]
    [Trait("Category", "Performance")]
    [Trait("Category", "Memory")]
    public void Content_catalog_reloads_meet_time_allocation_and_retained_memory_budgets()
    {
        var warmup = new ContentRepository();
        Assert.NotEmpty(warmup.Formulas.Articles);
        Assert.NotEmpty(warmup.Chapters.Chapters);
        Assert.NotEmpty(warmup.Exam.Exercises);

        ForceFullCollection();
        var retainedBefore = GC.GetTotalMemory(forceFullCollection: true);
        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();
        var formulas = 0;
        var chapters = 0;
        var exercises = 0;

        for (var index = 0; index < ContentReloads; index++)
        {
            var content = new ContentRepository();
            formulas += content.Formulas.Articles.Count;
            chapters += content.Chapters.Chapters.Count;
            exercises += content.Exam.Exercises.Count;
        }

        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        ForceFullCollection();
        var retainedAfter = GC.GetTotalMemory(forceFullCollection: true);
        var retainedGrowthBytes = Math.Max(0, retainedAfter - retainedBefore);

        ReportMetric(
            "content-reload",
            $"reloads={ContentReloads}; formulas={formulas}; chapters={chapters}; exercises={exercises}; " +
            $"elapsedMs={stopwatch.Elapsed.TotalMilliseconds:F1}; allocatedBytes={allocatedBytes}; " +
            $"retainedGrowthBytes={retainedGrowthBytes}");

        Assert.Equal(ContentReloads * warmup.Formulas.Articles.Count, formulas);
        Assert.Equal(ContentReloads * warmup.Chapters.Chapters.Count, chapters);
        Assert.Equal(ContentReloads * warmup.Exam.Exercises.Count, exercises);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(15),
            $"Ponowne odczytanie katalogu treści {ContentReloads} razy trwało {stopwatch.Elapsed}.");
        Assert.True(
            allocatedBytes <= 64L * 1024L * 1024L,
            $"Ponowne odczytanie katalogu zaalokowało {allocatedBytes} B.");
        Assert.True(
            retainedGrowthBytes <= 32L * 1024L * 1024L,
            $"Po pełnym GC pozostało {retainedGrowthBytes} B dodatkowej pamięci zarządzanej.");
    }

    [Fact]
    [Trait("Category", "Load")]
    [Trait("Category", "Memory")]
    public void Progress_store_reads_large_history_within_resource_budgets()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var factory = new AppDbContextFactory(Path.Combine(directory, "load.db"));

        try
        {
            var profileId = SeedProgressHistory(factory, ProgressRecordCount);
            Assert.Equal(ProgressRecordCount, ReadProgress(factory, profileId).Count);

            ForceFullCollection();
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            var totalRead = 0;
            for (var iteration = 0; iteration < ProgressReadCount; iteration++)
                totalRead += ReadProgress(factory, profileId).Count;
            stopwatch.Stop();
            var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
            var databaseBytes = GetDatabaseFootprint(factory.DatabasePath);

            ReportMetric(
                "progress-store",
                $"records={ProgressRecordCount}; reads={ProgressReadCount}; totalRead={totalRead}; " +
                $"elapsedMs={stopwatch.Elapsed.TotalMilliseconds:F1}; allocatedBytes={allocatedBytes}; " +
                $"databaseFootprintBytes={databaseBytes}");

            Assert.Equal(ProgressRecordCount * ProgressReadCount, totalRead);
            Assert.True(
                stopwatch.Elapsed < TimeSpan.FromSeconds(15),
                $"Odczyt {ProgressReadCount} historii po {ProgressRecordCount} wpisów trwał {stopwatch.Elapsed}.");
            Assert.True(
                allocatedBytes <= 64L * 1024L * 1024L,
                $"Odczyt historii postępu zaalokował {allocatedBytes} B.");
            Assert.True(
                databaseBytes <= 16L * 1024L * 1024L,
                $"Baza z {ProgressRecordCount} wpisami postępu zajmuje {databaseBytes} B.");
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    private static void RunCalculatorCases(ExpressionCalculator calculator, IReadOnlyList<CalculatorCase> cases, int iterations)
    {
        for (var iteration = 0; iteration < iterations; iteration++)
            foreach (var testCase in cases)
            {
                var result = calculator.Evaluate(testCase.Expression);
                if (!result.Success || result.Value is not double actual || Math.Abs(actual - testCase.Expected) > 1E-12d)
                {
                    throw new XunitException(
                        $"{testCase.Expression}: {result.ErrorCode}: {result.Message}: {result.Value}");
                }
            }
    }

    private static Guid SeedProgressHistory(AppDbContextFactory factory, int recordCount)
    {
        var profileId = Guid.NewGuid();
        using var context = factory.CreateDbContext();
        context.Database.Migrate();
        context.Profiles.Add(new LocalProfileEntity
        {
            Id = profileId,
            DisplayName = "Obciążenie",
            NormalizedName = "OBCIAZENIE",
            Kind = ProfileKind.Guest,
            CreatedUtc = DateTime.UtcNow
        });
        context.ExerciseProgress.AddRange(
            Enumerable.Range(1, recordCount).Select(number => new ExerciseProgressEntity
            {
                ProfileId = profileId,
                ExerciseId = $"load-{number:D5}",
                CompletedUtc = DateTime.UtcNow
            }));
        context.SaveChanges();
        return profileId;
    }

    private static HashSet<string> ReadProgress(AppDbContextFactory factory, Guid profileId)
    {
        using var context = factory.CreateDbContext();
        return context.ExerciseProgress
            .AsNoTracking()
            .Where(item => item.ProfileId == profileId)
            .Select(item => item.ExerciseId)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static long GetDatabaseFootprint(string databasePath) =>
        new[] { databasePath, databasePath + "-wal", databasePath + "-shm" }
            .Where(File.Exists)
            .Sum(path => new FileInfo(path).Length);

    private static void ForceFullCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private static void ReportMetric(string scenario, string values) =>
        TestContext.Current.TestOutputHelper?.WriteLine(
            string.Format(CultureInfo.InvariantCulture, "METRIC {0}: {1}", scenario, values));

    private sealed record CalculatorCase(string Expression, double Expected);
}
