using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abituria.Services;

public sealed record AppBuildInfo(
    string Version,
    string Commit,
    string License,
    string Author,
    string RepositoryUrl)
{
    public const string ProjectLicense = "MIT";
    public const string ProjectAuthor = "Adam Kubiś";
    public const string ProjectRepositoryUrl = "https://github.com/haribo841/Abituria";
    public const string UnknownCommit = "nieznany";

    public static AppBuildInfo Current { get; } = FromAssembly(typeof(AppBuildInfo).Assembly);

    public static AppBuildInfo FromAssembly(Assembly assembly, string? commitOverride = null)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        var version = ResolveVersion(informationalVersion, assembly.GetName().Version);
        var metadata = assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .GroupBy(attribute => attribute.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last().Value, StringComparer.OrdinalIgnoreCase);
        var commit = FirstNonEmpty(
            commitOverride,
            Metadata(metadata, "RepositoryCommit"),
            Metadata(metadata, "SourceRevisionId"),
            ExtractCommit(informationalVersion),
            Environment.GetEnvironmentVariable("ABITURIA_COMMIT"),
            Environment.GetEnvironmentVariable("GITHUB_SHA"),
            UnknownCommit);

        return new AppBuildInfo(version, commit, ProjectLicense, ProjectAuthor, ProjectRepositoryUrl);
    }

    private static string ResolveVersion(string? informationalVersion, Version? assemblyVersion)
    {
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            var metadataSeparator = informationalVersion.IndexOf('+', StringComparison.Ordinal);
            return metadataSeparator < 0
                ? informationalVersion
                : informationalVersion[..metadataSeparator];
        }

        return assemblyVersion?.ToString(3) ?? "0.0.0";
    }

    private static string? Metadata(IReadOnlyDictionary<string, string?> metadata, string key) =>
        metadata.TryGetValue(key, out var value) ? value : null;

    private static string? ExtractCommit(string? informationalVersion)
    {
        if (string.IsNullOrWhiteSpace(informationalVersion)) return null;
        var separator = informationalVersion.IndexOf('+', StringComparison.Ordinal);
        if (separator < 0 || separator == informationalVersion.Length - 1) return null;

        return informationalVersion[(separator + 1)..]
            .Split(['.', '-', '/'], StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault(IsCommitHash);
    }

    private static bool IsCommitHash(string value) =>
        value.Length is >= 7 and <= 64 && value.All(Uri.IsHexDigit);

    private static string FirstNonEmpty(params string?[] values) =>
        values.First(value => !string.IsNullOrWhiteSpace(value))!;
}
