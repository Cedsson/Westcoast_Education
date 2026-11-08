using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WestcoastEducation.Services;

namespace WestcoastEducation.ViewModels;

//Huvudfönstrets ViewModel: navigering mellan sidorna och temabyte.</summary>
public partial class MainWindowViewModel : ViewModelBase
{
    public CoursesViewModel Courses { get; }
    public StudentsViewModel Students { get; }
    public TeachersViewModel Teachers { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCoursesActive), nameof(IsStudentsActive), nameof(IsTeachersActive))]
    private PageViewModelBase _currentPage;

    public bool IsCoursesActive => CurrentPage == Courses;
    public bool IsStudentsActive => CurrentPage == Students;
    public bool IsTeachersActive => CurrentPage == Teachers;

    [ObservableProperty]
    private bool _isDarkTheme;

    public MainWindowViewModel(IRestDbService db)
    {
        Courses = new CoursesViewModel(db);
        Students = new StudentsViewModel(db);
        Teachers = new TeachersViewModel(db);
        _currentPage = Courses;
        _ = Courses.EnsureLoadedAsync();
    }

    [RelayCommand]
    private void ShowCourses() => Navigate(Courses);

    [RelayCommand]
    private void ShowStudents() => Navigate(Students);

    [RelayCommand]
    private void ShowTeachers() => Navigate(Teachers);

    private void Navigate(PageViewModelBase page)
    {
        CurrentPage = page;
        _ = page.EnsureLoadedAsync();
    }

    //Byter mellan ljust och mörkt tema. Alla färger är DynamicResource och uppdateras direkt.</summary>
    partial void OnIsDarkThemeChanged(bool value)
    {
        if (Application.Current != null)
            Application.Current.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
    }
}
