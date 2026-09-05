using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Abituria.Tests;

public sealed class ContentProvenanceTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string[] DistributionStatuses = ["approved", "blocked"];
    private static readonly string[] NewlyApprovedGroupIds =
    [
        "cke-2018-main-basic-exam",
        "cke-2018-main-extended-exam",
        "cke-2018-correction-basic-exam",
        "cke-2019-main-basic-exam",
        "cke-2019-main-extended-exam",
        "cke-2019-correction-basic-exam",
        "cke-2020-main-basic-exam",
        "cke-2020-main-extended-exam",
        "cke-2020-correction-basic-exam",
        "cke-2021-main-basic-exam",
        "cke-2021-main-extended-exam",
        "cke-2022-main-basic-exam",
        "cke-2022-main-extended-exam",
        "cke-2022-correction-basic-exam",
        "cke-2023-main-basic-exam",
        "cke-2023-correction-basic-exam",
        "cke-2024-correction-basic-exam",
        "cke-2025-correction-basic-exam"
    ];
    private static readonly string[] BlockedGroupIds =
    [
        "cke-2016-main-basic-exam",
        "cke-2016-main-extended-exam",
        "cke-2016-correction-basic-exam",
        "cke-2017-main-basic-exam",
        "cke-2017-main-extended-exam",
        "cke-2017-correction-basic-exam",
        "cke-2015-main-basic-exam",
        "cke-2015-main-extended-exam",
        "cke-2015-correction-basic-exam",
        "cke-2023-f2015-main-basic-exam",
        "cke-2023-f2015-main-extended-exam",
        "cke-2023-f2015-correction-basic-exam",
        "cke-2024-f2015-main-basic-exam",
        "cke-2024-f2015-main-extended-exam",
        "cke-2024-f2015-correction-basic-exam",
        "cke-2025-f2015-main-basic-exam",
        "cke-2025-f2015-main-extended-exam",
        "cke-2025-f2015-correction-basic-exam",
        "cke-2026-f2015-main-basic-exam",
        "cke-2026-f2015-main-extended-exam",
        "runtime-vector-diagrams"
    ];
    private static readonly JsonSerializerOptions ManifestJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void Every_packaged_content_font_and_image_has_exactly_one_provenance_record()
    {
        var manifest = ReadManifest();
        Assert.Equal(1, manifest.SchemaVersion);

        var packaged = ReadPackagedResourcePatterns()
            .SelectMany(ExpandGlob)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var declarations = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var group in manifest.Assets)
        {
            Assert.False(string.IsNullOrWhiteSpace(group.Id));
            Assert.False(string.IsNullOrWhiteSpace(group.Author));
            Assert.False(string.IsNullOrWhiteSpace(group.Source));
            Assert.False(string.IsNullOrWhiteSpace(group.License));
            Assert.Contains(group.DistributionStatus, DistributionStatuses);
            Assert.NotEmpty(group.Evidence);
            Assert.All(group.Evidence, evidence =>
                Assert.True(File.Exists(Absolute(evidence)), $"Brak dowodu dla {group.Id}: {evidence}"));

            if (group.DistributionStatus == "blocked")
                Assert.False(string.IsNullOrWhiteSpace(group.BlockedReason));

            Assert.NotEmpty(group.Paths);
            foreach (var pattern in group.Paths)
            {
                var matches = ExpandGlob(pattern).ToArray();
                Assert.NotEmpty(matches);
                foreach (var match in matches)
                    Assert.True(declarations.TryAdd(match, group.Id), $"{match} występuje w więcej niż jednej grupie.");
            }
        }

        Assert.Equal(packaged, declarations.Keys.Order(StringComparer.Ordinal));
    }

    [Fact]
    public void Public_release_flag_matches_all_distribution_statuses()
    {
        var manifest = ReadManifest();
        var blockers = manifest.Assets.Where(asset => asset.DistributionStatus == "blocked").ToArray();

        Assert.Equal(blockers.Length == 0, manifest.ReleaseEligible);
        Assert.All(blockers, blocker => Assert.False(string.IsNullOrWhiteSpace(blocker.BlockedReason)));
    }

    [Fact]
    public void Owner_declaration_preserves_approved_groups_and_blocks_every_unconfirmed_exam_scope()
    {
        var manifest = ReadManifest();
        var declaration = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var approvedGroups = manifest.Assets
            .Where(asset => NewlyApprovedGroupIds.Contains(asset.Id, StringComparer.Ordinal))
            .ToArray();
        var blockedGroups = manifest.Assets
            .Where(asset => BlockedGroupIds.Contains(asset.Id, StringComparer.Ordinal))
            .ToArray();

        Assert.False(manifest.ReleaseEligible);
        Assert.Equal(18, approvedGroups.Length);
        Assert.All(approvedGroups, group =>
        {
            Assert.Equal("approved", group.DistributionStatus);
            Assert.Contains(group.Id, declaration, StringComparison.Ordinal);
        });
        Assert.Equal(21, blockedGroups.Length);
        Assert.All(blockedGroups, group =>
        {
            Assert.Equal("blocked", group.DistributionStatus);
            Assert.False(string.IsNullOrWhiteSpace(group.BlockedReason));
        });
        Assert.Contains("1 września 2026 r.", declaration, StringComparison.Ordinal);
    }

    private static ProvenanceManifest ReadManifest() => JsonSerializer.Deserialize<ProvenanceManifest>(
        File.ReadAllText(Absolute("Content/provenance.json")),
        ManifestJsonOptions)
        ?? throw new InvalidDataException("Nie można odczytać manifestu pochodzenia.");

    private static IEnumerable<string> ReadPackagedResourcePatterns()
    {
        var project = XDocument.Load(Absolute("Abituria.csproj"));
        var patterns = project.Descendants("AvaloniaResource")
            .SelectMany(node => ((string?)node.Attribute("Include") ?? string.Empty)
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        var icons = project.Descendants("ApplicationIcon")
            .Select(node => node.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value));
        return patterns.Concat(icons);
    }

    private static IEnumerable<string> ExpandGlob(string pattern)
    {
        var normalized = pattern.Replace('\\', '/');
        var topDirectory = normalized.Split('/')[0];
        var searchRoot = Absolute(topDirectory);
        if (!Directory.Exists(searchRoot)) return [];

        var expression = "^" + Regex.Escape(normalized)
            .Replace(@"\*\*/", "(?:.*/)?", StringComparison.Ordinal)
            .Replace(@"\*", "[^/]*", StringComparison.Ordinal)
            .Replace(@"\?", "[^/]", StringComparison.Ordinal) + "$";
        var regex = new Regex(expression, RegexOptions.CultureInvariant);
        return Directory.EnumerateFiles(searchRoot, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(RepositoryRoot, path).Replace('\\', '/'))
            .Where(path => regex.IsMatch(path))
            .Order(StringComparer.Ordinal);
    }

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.csproj")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium.");
    }

    private sealed record ProvenanceManifest(int SchemaVersion, bool ReleaseEligible, List<ProvenanceAsset> Assets);

    private sealed record ProvenanceAsset(
        string Id,
        List<string> Paths,
        string Author,
        string Source,
        string License,
        string DistributionStatus,
        string? BlockedReason,
        List<string> Evidence);
}
