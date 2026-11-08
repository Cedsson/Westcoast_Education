using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WestcoastEducation.Models;
using WestcoastEducation.Services;

namespace WestcoastEducation.ViewModels;

//Listar, lägger till och raderar kurser.</summary>
public partial class CoursesViewModel : PageViewModelBase
{
    private readonly IRestDbService _db;

    public CoursesViewModel(IRestDbService db) => _db = db;

    public override string Title => "Kurser";

    public ObservableCollection<Course> Courses { get; } = new();

    public IReadOnlyList<CourseType> CourseTypes { get; } = Enum.GetValues<CourseType>();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCourseCommand))]
    private Course? _selectedCourse;

    //Formulär för ny kurs
    [ObservableProperty] private string _courseNumber = "";
    [ObservableProperty] private string _courseTitle = "";
    [ObservableProperty] private DateTimeOffset? _startDate = DateTimeOffset.Now.Date;
    [ObservableProperty] private DateTimeOffset? _endDate = DateTimeOffset.Now.Date.AddDays(30);
    [ObservableProperty] private CourseType _selectedType = CourseType.Klassrum;

    protected override Task LoadAsync() => RunAsync(async () =>
    {
        var courses = await _db.GetAllAsync<Course>(Collections.Courses);
        Courses.Clear();
        foreach (var course in courses.OrderBy(c => c.StartDate))
            Courses.Add(course);
        ShowInfo($"{Courses.Count} kurser hämtade.");
    });

    [RelayCommand]
    private Task Refresh() => LoadAsync();

    [RelayCommand]
    private async Task AddCourse()
    {
        var number = CourseNumber.Trim();
        var title = CourseTitle.Trim();

        if (string.IsNullOrEmpty(number) || string.IsNullOrEmpty(title))
        {
            ShowError("Kursnummer och titel måste anges.");
            return;
        }
        if (StartDate == null || EndDate == null)
        {
            ShowError("Start- och slutdatum måste anges.");
            return;
        }
        if (EndDate.Value.Date < StartDate.Value.Date)
        {
            ShowError("Slutdatum får inte vara före startdatum.");
            return;
        }
        if (Courses.Any(c => string.Equals(c.CourseNumber, number, StringComparison.OrdinalIgnoreCase)))
        {
            ShowError($"Kursnummer {number} finns redan.");
            return;
        }

        var course = new Course
        {
            CourseNumber = number,
            Title = title,
            StartDate = StartDate.Value.Date,
            EndDate = EndDate.Value.Date,
            Type = SelectedType
        };

        await RunAsync(async () =>
        {
            var saved = await _db.AddAsync(Collections.Courses, course);
            Courses.Add(saved);
            CourseNumber = "";
            CourseTitle = "";
        }, $"Kursen {title} lades till.");
    }

    private bool CanDeleteCourse() => SelectedCourse != null;

    [RelayCommand(CanExecute = nameof(CanDeleteCourse))]
    private async Task DeleteCourse()
    {
        var course = SelectedCourse;
        if (course?.Id == null)
            return;

        await RunAsync(async () =>
        {
            await _db.DeleteAsync(Collections.Courses, course.Id);
            Courses.Remove(course);
            SelectedCourse = null;
        }, $"Kursen {course.Title} raderades.");
    }
}
