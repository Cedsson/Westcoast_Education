using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WestcoastEducation.Models;
using WestcoastEducation.Services;

namespace WestcoastEducation.ViewModels;

//Listar och lägger till studenter.</summary>
public partial class StudentsViewModel : PageViewModelBase
{
    private readonly IRestDbService _db;

    public StudentsViewModel(IRestDbService db) => _db = db;

    public override string Title => "Studenter";

    public ObservableCollection<Student> Students { get; } = new();

    //Formulär för ny student
    [ObservableProperty] private string _firstName = "";
    [ObservableProperty] private string _lastName = "";
    [ObservableProperty] private string _personalNumber = "";
    [ObservableProperty] private string _phone = "";
    [ObservableProperty] private string _street = "";
    [ObservableProperty] private string _postalCode = "";
    [ObservableProperty] private string _city = "";

    protected override Task LoadAsync() => RunAsync(async () =>
    {
        var students = await _db.GetAllAsync<Student>(Collections.Students);
        Students.Clear();
        foreach (var student in students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName))
            Students.Add(student);
        ShowInfo($"{Students.Count} studenter hämtade.");
    });

    [RelayCommand]
    private Task Refresh() => LoadAsync();

    [RelayCommand]
    private async Task AddStudent()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ShowError("Förnamn och efternamn måste anges.");
            return;
        }
        if (string.IsNullOrWhiteSpace(PersonalNumber))
        {
            ShowError("Personnummer måste anges.");
            return;
        }
        if (Students.Any(s => s.PersonalNumber == PersonalNumber.Trim()))
        {
            ShowError($"Personnummer {PersonalNumber.Trim()} finns redan registrerat.");
            return;
        }

        var student = new Student
        {
            FirstName = FirstName.Trim(),
            LastName = LastName.Trim(),
            PersonalNumber = PersonalNumber.Trim(),
            Phone = Phone.Trim(),
            Street = Street.Trim(),
            PostalCode = PostalCode.Trim(),
            City = City.Trim()
        };

        await RunAsync(async () =>
        {
            var saved = await _db.AddAsync(Collections.Students, student);
            Students.Add(saved);
            FirstName = LastName = PersonalNumber = Phone = Street = PostalCode = City = "";
        }, $"Studenten {student.FullName} lades till.");
    }
}
