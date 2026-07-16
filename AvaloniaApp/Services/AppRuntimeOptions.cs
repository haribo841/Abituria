using System;
using System.IO;

namespace Abituria.Services;

public sealed record AppRuntimeOptions(
    string? DatabasePath,
    bool ImportLegacyProfiles,
    bool ShowMainWindow,
    bool IsReleaseSmokeTest)
{
    public const string ReleaseSmokeDatabaseFileName = "abituria-release-smoke.db";

    public static AppRuntimeOptions Desktop { get; } = new(
        DatabasePath: null,
        ImportLegacyProfiles: true,
        ShowMainWindow: true,
        IsReleaseSmokeTest: false);

    public static AppRuntimeOptions ReleaseSmokeTest(string dataDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        var fullPath = Path.GetFullPath(dataDirectory);
        var productionDataPath = Path.GetFullPath(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Abituria"));
        var comparison = OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        var canonicalPath = ResolveDirectoryLinks(fullPath);
        var canonicalProductionPath = ResolveDirectoryLinks(productionDataPath);
        if (IsSameOrChildPath(canonicalPath, canonicalProductionPath, comparison))
        {
            throw new ArgumentException(
                "Katalog smoke testu nie może być produkcyjnym katalogiem danych Abiturii ani jego podkatalogiem.",
                nameof(dataDirectory));
        }

        if (Directory.Exists(fullPath) && Directory.EnumerateFileSystemEntries(fullPath).Any())
        {
            throw new ArgumentException(
                "Katalog smoke testu nie jest pusty. Użyj nowego pustego katalogu.",
                nameof(dataDirectory));
        }

        var databasePath = Path.Combine(fullPath, ReleaseSmokeDatabaseFileName);
        return new AppRuntimeOptions(
            databasePath,
            ImportLegacyProfiles: false,
            ShowMainWindow: false,
            IsReleaseSmokeTest: true);
    }

    private static bool IsSameOrChildPath(
        string candidatePath,
        string parentPath,
        StringComparison comparison)
    {
        var candidate = Path.TrimEndingDirectorySeparator(candidatePath);
        var parent = Path.TrimEndingDirectorySeparator(parentPath);
        if (string.Equals(candidate, parent, comparison)) return true;

        var parentPrefix = parent + Path.DirectorySeparatorChar;
        return candidate.StartsWith(parentPrefix, comparison);
    }

    private static string ResolveDirectoryLinks(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var root = Path.GetPathRoot(fullPath) ??
            throw new ArgumentException("Ścieżka katalogu nie ma prawidłowego katalogu głównego.", nameof(path));
        var current = root;
        var relativePath = fullPath[root.Length..];
        foreach (var segment in relativePath.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries))
        {
            current = Path.Combine(current, segment);
            if (!Directory.Exists(current)) continue;

            var target = new DirectoryInfo(current).ResolveLinkTarget(returnFinalTarget: true);
            if (target is not null) current = target.FullName;
        }

        return Path.GetFullPath(current);
    }
}
