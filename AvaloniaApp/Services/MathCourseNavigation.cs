using Abituria.Models;
using Abituria.ViewModels;

namespace Abituria.Services;

public static class MathCourseNavigation
{
    public static MathCourseLesson[] GetVisibleLessons(
        MathCourseCatalog catalog,
        string areaId,
        CourseLevelFilter level) => catalog.Lessons
        .Where(lesson => string.Equals(lesson.AreaId, areaId, StringComparison.Ordinal))
        .Where(lesson => IsVisible(lesson, level))
        .OrderBy(lesson => lesson.Order)
        .ToArray();

    public static CourseRequirement[] GetVisibleRequirements(
        MathCourseCatalog catalog,
        string areaId,
        CourseLevelFilter level) => catalog.Requirements
        .Where(requirement => string.Equals(requirement.AreaId, areaId, StringComparison.Ordinal))
        .Where(requirement => level == CourseLevelFilter.Extended ||
            string.Equals(requirement.Level, "basic", StringComparison.Ordinal))
        .OrderBy(requirement => requirement.Level == "basic" ? 0 : 1)
        .ThenBy(requirement => requirement.Number)
        .ToArray();

    public static bool IsVisible(MathCourseLesson lesson, CourseLevelFilter level) =>
        lesson.AlwaysVisible ||
        level == CourseLevelFilter.Extended ||
        string.Equals(lesson.Level, "basic", StringComparison.Ordinal);
}
