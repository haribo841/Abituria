using System;
using System.Collections.Generic;
using System.Linq;
using Abituria.Models;

namespace Abituria.Services;

public static class OfficialCourseExampleCatalogValidator
{
    private static readonly HashSet<string> SupportedLevels =
        new(["basic", "extended"], StringComparer.Ordinal);

    public static void Validate(OfficialCourseExampleCatalog catalog, MathCourseCatalog course)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(course);
        if (catalog.SchemaVersion != 1)
            throw new InvalidOperationException($"Nieobsługiwany schemat przykładów CKE: {catalog.SchemaVersion}.");
        if (catalog.Sources.Count == 0 || catalog.Examples.Count == 0)
            throw new InvalidOperationException("Katalog przykładów CKE nie może być pusty.");

        RequireUnique(catalog.Sources.Select(item => item.Id), "źródła przykładów CKE");
        RequireUnique(catalog.Examples.Select(item => item.Id), "przykładu CKE");
        var courseRequirements = course.Requirements.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
        var courseSources = course.Sources.ToDictionary(item => item.Id, StringComparer.Ordinal);

        foreach (var source in catalog.Sources)
            ValidateSource(source, courseSources, catalog.Examples);
        foreach (var example in catalog.Examples)
            ValidateExample(example, catalog.Sources, courseRequirements);
    }

    private static void ValidateSource(
        OfficialCourseExampleSource source,
        Dictionary<string, CourseSourceDocument> courseSources,
        IReadOnlyCollection<OfficialCourseExample> examples)
    {
        if (string.IsNullOrWhiteSpace(source.Id) || string.IsNullOrWhiteSpace(source.Title) ||
            string.IsNullOrWhiteSpace(source.Publisher) || string.IsNullOrWhiteSpace(source.DocumentUrl) ||
            string.IsNullOrWhiteSpace(source.VerifiedOn) || !SupportedLevels.Contains(source.Level))
            throw new InvalidOperationException("Źródło przykładów CKE ma niepełne metadane.");
        if (source.DocumentSha256.Length != 64 || !source.DocumentSha256.All(Uri.IsHexDigit))
            throw new InvalidOperationException($"Źródło '{source.Id}' ma nieprawidłową sumę SHA-256.");
        if (source.FirstExamplePage <= 0 || source.LastExamplePage < source.FirstExamplePage || source.ExampleCount <= 0)
            throw new InvalidOperationException($"Źródło '{source.Id}' ma nieprawidłowy zakres przykładów.");
        if (!courseSources.TryGetValue(source.Id, out var courseSource) ||
            !string.Equals(courseSource.DocumentUrl, source.DocumentUrl, StringComparison.Ordinal) ||
            !string.Equals(courseSource.DocumentSha256, source.DocumentSha256, StringComparison.Ordinal) ||
            !string.Equals(courseSource.VerifiedOn, source.VerifiedOn, StringComparison.Ordinal))
            throw new InvalidOperationException($"Źródło '{source.Id}' nie odpowiada przypiętemu źródłu kursu.");
        if (examples.Count(item => string.Equals(item.SourceId, source.Id, StringComparison.Ordinal)) != source.ExampleCount)
            throw new InvalidOperationException($"Źródło '{source.Id}' nie ma deklarowanej liczby przykładów.");
    }

    private static void ValidateExample(
        OfficialCourseExample example,
        IReadOnlyCollection<OfficialCourseExampleSource> sources,
        HashSet<string> courseRequirements)
    {
        var source = sources.SingleOrDefault(item => string.Equals(item.Id, example.SourceId, StringComparison.Ordinal));
        if (source is null || !string.Equals(source.Level, example.Level, StringComparison.Ordinal))
            throw new InvalidOperationException($"Przykład '{example.Id}' wskazuje nieznane albo niewłaściwe źródło.");
        if (string.IsNullOrWhiteSpace(example.Id) || string.IsNullOrWhiteSpace(example.OfficialNumber) ||
            string.IsNullOrWhiteSpace(example.Transcription) || !SupportedLevels.Contains(example.Level) ||
            example.Order <= 0 || example.MaximumPoints <= 0)
            throw new InvalidOperationException("Przykład CKE ma niepełne metadane albo treść.");
        if (example.SourcePages.Count == 0 || example.SourcePages.Distinct().Count() != example.SourcePages.Count ||
            !example.SourcePages.SequenceEqual(example.SourcePages.Order()) ||
            example.SourcePages.Any(page => page < source.FirstExamplePage || page > source.LastExamplePage))
            throw new InvalidOperationException($"Przykład '{example.Id}' ma nieprawidłowe strony źródłowe.");
        if (example.RequirementIds.Count == 0 || example.RequirementIds.Distinct(StringComparer.Ordinal).Count() != example.RequirementIds.Count ||
            example.RequirementIds.Any(id => !courseRequirements.Contains(id)))
            throw new InvalidOperationException($"Przykład '{example.Id}' ma nieprawidłowe mapowanie wymagań.");
        if (example.VisualReferences.Any(reference => string.IsNullOrWhiteSpace(reference.AlternativeText) ||
            !example.SourcePages.Contains(reference.SourcePage)))
            throw new InvalidOperationException($"Przykład '{example.Id}' ma nieprawidłowy opis figury.");
    }

    private static void RequireUnique(IEnumerable<string> values, string label)
    {
        var duplicate = values.GroupBy(value => value, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() != 1);
        if (duplicate is not null)
            throw new InvalidOperationException($"Powtórzony identyfikator {label}: {duplicate.Key}.");
    }
}
