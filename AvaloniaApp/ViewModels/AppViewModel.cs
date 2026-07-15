using Abituria.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Abituria.ViewModels;

public enum AppPage
{
    Login,
    Home,
    Formulas,
    FormulaDetail,
    Exams,
    ExerciseList,
    Exercise,
    Chapters,
    ChapterDetail,
    Calculator,
    GeneralCalculator,
    Roadmap,
    About,
    Profile,
    Placeholder
}

public sealed class AppViewModel : ObservableObject
{
    private AppPage _currentPage = AppPage.Login;
    private LocalProfile? _activeProfile;
    private FormulaArticle? _selectedFormula;
    private ChapterArticle? _selectedChapter;
    private ExerciseDefinition? _selectedExercise;
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
    public ChapterArticle? SelectedChapter { get => _selectedChapter; private set => SetProperty(ref _selectedChapter, value); }
    public ExerciseDefinition? SelectedExercise { get => _selectedExercise; private set => SetProperty(ref _selectedExercise, value); }
    public string? SelectedTopicId { get => _selectedTopicId; private set => SetProperty(ref _selectedTopicId, value); }
    public string? SelectedRoadmapId { get => _selectedRoadmapId; private set => SetProperty(ref _selectedRoadmapId, value); }
    public PlaceholderItem? SelectedPlaceholder { get => _selectedPlaceholder; private set => SetProperty(ref _selectedPlaceholder, value); }

    public void Login(LocalProfile profile)
    {
        ActiveProfile = profile;
        CurrentPage = AppPage.Home;
    }

    public void Logout()
    {
        ActiveProfile = null;
        CurrentPage = AppPage.Login;
    }

    public void Navigate(AppPage page) => CurrentPage = ActiveProfile is null ? AppPage.Login : page;
    public void OpenFormula(FormulaArticle article) { SelectedFormula = article; CurrentPage = AppPage.FormulaDetail; }
    public void OpenChapter(ChapterArticle chapter) { SelectedChapter = chapter; CurrentPage = AppPage.ChapterDetail; }
    public void OpenExercise(ExerciseDefinition exercise) { SelectedExercise = exercise; CurrentPage = AppPage.Exercise; }
    public void OpenExam() { SelectedTopicId = null; CurrentPage = AppPage.ExerciseList; }
    public void OpenTopic(string topicId) { SelectedTopicId = topicId; CurrentPage = AppPage.ExerciseList; }
    public void OpenGeneralCalculator() => CurrentPage = AppPage.GeneralCalculator;
    public void OpenRoadmap(string? itemId = null) { SelectedRoadmapId = itemId; CurrentPage = AppPage.Roadmap; }
    public void OpenPlaceholder(PlaceholderItem item) { SelectedPlaceholder = item; CurrentPage = AppPage.Placeholder; }
}
