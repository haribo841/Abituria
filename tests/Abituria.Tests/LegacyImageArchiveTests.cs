using System.Security.Cryptography;
using System.Xml.Linq;

namespace Abituria.Tests;

public sealed class LegacyImageArchiveTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Archive_contains_75_mapped_files_with_matching_sha256_sums()
    {
        var archiveRoot = Absolute("docs/legacy/originals/images");
        var metadataNames = new HashSet<string>(StringComparer.Ordinal)
            { "README.md", "PATH-MAPPING.csv", "SHA256SUMS" };
        var files = Directory.GetFiles(archiveRoot, "*", SearchOption.AllDirectories)
            .Where(path => !metadataNames.Contains(Path.GetFileName(path)))
            .Order(StringComparer.Ordinal)
            .ToArray();
        var checksumLines = File.ReadAllLines(Path.Combine(archiveRoot, "SHA256SUMS"));
        var mappingLines = File.ReadAllLines(Path.Combine(archiveRoot, "PATH-MAPPING.csv"));

        Assert.Equal(75, files.Length);
        Assert.Equal(75, checksumLines.Length);
        Assert.Equal(76, mappingLines.Length);
        Assert.Equal("oldPath,archivePath", mappingLines[0]);

        var mappings = mappingLines.Skip(1).ToHashSet(StringComparer.Ordinal);
        foreach (var checksumLine in checksumLines)
        {
            Assert.True(checksumLine.Length > 66, checksumLine);
            Assert.Equal("  ", checksumLine.Substring(64, 2));
            var expectedHash = checksumLine[..64];
            var relativePath = checksumLine[66..];
            _ = Convert.FromHexString(expectedHash);
            var absolutePath = Path.GetFullPath(Path.Combine(
                archiveRoot,
                relativePath.Replace('/', Path.DirectorySeparatorChar)));
            Assert.StartsWith(
                Path.GetFullPath(archiveRoot) + Path.DirectorySeparatorChar,
                absolutePath,
                StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(absolutePath), relativePath);
            var actualHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(absolutePath)));
            Assert.Equal(expectedHash, actualHash);
            Assert.Contains(
                $"\"img/{relativePath}\",\"docs/legacy/originals/images/{relativePath}\"",
                mappings);
        }
    }

    [Fact]
    public void Runtime_project_and_content_do_not_reference_raster_images()
    {
        var project = XDocument.Load(Absolute("Abituria.csproj"));
        var resources = project.Descendants("AvaloniaResource")
            .Select(node => (string?)node.Attribute("Include") ?? string.Empty)
            .ToArray();
        var applicationIcon = project.Descendants("ApplicationIcon").Single().Value.Replace('\\', '/');
        var productionSource = string.Join('\n',
            Directory.GetFiles(Absolute("AvaloniaApp"), "*", SearchOption.AllDirectories)
                .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                .Select(File.ReadAllText));
        var content = string.Join('\n', Directory.GetFiles(Absolute("Content"), "*.json").Select(File.ReadAllText));

        Assert.DoesNotContain(resources, resource => resource.Contains("img", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("img/icon.ico", applicationIcon);
        Assert.True(File.Exists(Absolute(applicationIcon)));
        foreach (var forbidden in new[] { "Bitmap", "AssetImage", "new Image", "<Image", ".png", ".jpg" })
            Assert.DoesNotContain(forbidden, productionSource, StringComparison.OrdinalIgnoreCase);
        foreach (var forbidden in new[] { "\"image\"", ".png", ".jpg" })
            Assert.DoesNotContain(forbidden, content, StringComparison.OrdinalIgnoreCase);
    }

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono repozytorium Abituria.");
    }
}
