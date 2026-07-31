using Abituria.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Abituria.ViewModels;

public enum AppPage
{
    Login,
    Home,
    Formulas,
    FormulaDetail,
    Matura,
    Tasks,
    ExerciseList,
    Exercise,
    Chapters,
    CourseArea,
    CourseLesson,
    Calculator,
    GeneralCalculator,
    Options,
    Roadmap,
    About,
    Profile,
    Placeholder
}

public enum CourseLevelFilter
{
    Basic,
    Extended
}

public enum ExamNavigationOrigin
{
    Matura,
    Tasks
}

public sealed class AppViewModel : ObservableObject
{
    private AppPage _currentPage = AppPage.Login;
    private LocalProfile? _activeProfile;
    private FormulaArticle? _selectedFormula;
    private CourseArea? _selectedCourseArea;
    private MathCourseLesson? _selectedCourseLesson;
    private LearningExercise? _selectedExercise;
    private CourseLevelFilter _selectedCourseLevel = CourseLevelFilter.Basic;
    private ExamNavigationOrigin _examNavigationOrigin = ExamNavigationOrigin.Matura;
    private CalculatorPipMode _calculatorPipMode = CalculatorPipMode.OwnedWindow;
    private string? _selectedTopicId;
    private string? _selectedRoadmapId;
    private PlaceholderItem? _selectedPlaceholder;

    public AppPage CurrentPage
    {
        get => _currentPage;
        private set => SetProperty(ref _currentPage, value);
    }

    public LocalProfile? ActiveProfile
    {
        get => _activeProfile;
        private set => SetProperty(ref _activeProfile, value);
    }

    public FormulaArticle? SelectedFormula { get => _selectedFormula; private set => SetProperty(ref _selectedFormula, value); }
    public CourseArea? SelectedCourseArea { get => _selectedCourseArea; private set => SetProperty(ref _selectedCourseArea, value); }
    public MathCourseLesson? SelectedCourseLesson { get => _selectedCourseLesson; private set => SetProperty(ref _selectedCourseLesson, value); }
    public LearningExercise? SelectedExercise { get => _selectedExercise; private set => SetProperty(ref _selectedExercise, value); }
    public CourseLevelFilter SelectedCourseLevel { get => _selectedCourseLevel; private set => SetProperty(ref _selectedCourseLevel, value); }
    public ExamNavigationOrigin ExamNavigationOrigin { get => _examNavigationOrigin; private set => SetProperty(ref _examNavigationOrigin, value); }
    public CalculatorPipMode CalculatorPipMode { get => _calculatorPipMode; private set => SetProperty(ref _calculatorPipMode, value); }
    public string? SelectedTopicId { get => _selectedTopicId; private set => SetProperty(ref _selectedTopicId, value); }
    public string? SelectedRoadmapId { get => _selectedRoadmapId; private set => SetProperty(ref _selectedRoadmapId, value); }
    public PlaceholderItem? SelectedPlaceholder { get => _selectedPlaceholder; private set => SetProperty(ref _selectedPlaceholder, value); }

    public void Login(LocalProfile profile)
    {
        ActiveProfile = profile;
        CalculatorPipMode = profile.CalculatorPipMode;
        CurrentPage = AppPage.Home;
    }

    public void Logout()
    {
        ActiveProfile = null;
        CalculatorPipMode = CalculatorPipMode.OwnedWindow;
        CurrentPage = AppPage.Login;
    }

    public void Navigate(AppPage page)
    {
        if (ActiveProfile is null)
        {
            CurrentPage = AppPage.Login;
            return;
        }

        if (page == AppPage.Matura) ExamNavigationOrigin = ExamNavigationOrigin.Matura;
        if (page == AppPage.Tasks) ExamNavigationOrigin = ExamNavigationOrigin.Tasks;
        CurrentPage = page;
    }
    public void OpenFormula(FormulaArticle article) { SelectedFormula = article; CurrentPage = AppPage.FormulaDetail; }
    public void OpenCourseArea(CourseArea area) { SelectedCourseArea = area; CurrentPage = AppPage.CourseArea; }
    public void OpenCourseLesson(MathCourseLesson lesson) { SelectedCourseLesson = lesson; CurrentPage = AppPage.CourseLesson; }
    public void SetCourseLevel(CourseLevelFilter level) => SelectedCourseLevel = level;
    public void OpenExercise(LearningExercise exercise)
    {
        SelectedCourseLesson = null;
        SelectedExercise = exercise;
        CurrentPage = AppPage.Exercise;
    }

    public void OpenCourseExercise(LearningExercise exercise)
    {
        SelectedExercise = exercise;
        CurrentPage = AppPage.Exercise;
    }

    public void OpenRandomExercise(LearningExercise exercise, string? topicId)
    {
        SelectedTopicId = topicId;
        ExamNavigationOrigin = topicId is null ? ExamNavigationOrigin.Matura : ExamNavigationOrigin.Tasks;
        OpenExercise(exercise);
    }
    public void OpenExam()
    {
        SelectedTopicId = null;
        ExamNavigationOrigin = ExamNavigationOrigin.Matura;
        CurrentPage = AppPage.ExerciseList;
    }
    public void OpenTopic(string topicId)
    {
        SelectedTopicId = topicId;
        ExamNavigationOrigin = ExamNavigationOrigin.Tasks;
        CurrentPage = AppPage.ExerciseList;
    }
    public void OpenGeneralCalculator() => CurrentPage = AppPage.GeneralCalculator;
    public void SetCalculatorPipMode(CalculatorPipMode mode) => CalculatorPipMode = mode;
    public void OpenRoadmap(string? itemId = null) { SelectedRoadmapId = itemId; CurrentPage = AppPage.Roadmap; }
    public void OpenPlaceholder(PlaceholderItem item) { SelectedPlaceholder = item; CurrentPage = AppPage.Placeholder; }
}
