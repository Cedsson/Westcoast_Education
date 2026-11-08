using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WestcoastEducation.Models;
using WestcoastEducation.Services;

namespace WestcoastEducation.ViewModels;

/// <summary>Listar och lägger till lärare samt hanterar en lärares kompetensområden.</summary>
public partial class TeachersViewModel : PageViewModelBase
{
    private readonly IRestDbService _db;

    public TeachersViewModel(IRestDbService db) => _db = db;

    public override string Title => "Lärare";

    public ObservableCollection<Teacher> Teachers { get; } = new();

    /// <summary>Kompetensområden för vald lärare.</summary>
    public ObservableCollection<string> SelectedExpertise { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedTeacher))]
    [NotifyCanExecuteChangedFor(nameof(AddExpertiseCommand))]
    private Teacher? _selectedTeacher;

    public bool HasSelectedTeacher => SelectedTeacher != null;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddExpertiseCommand))]
    private string _newExpertise = "";

    // --- Formulär för ny lärare ---
    [ObservableProperty] private string _firstName = "";
    [ObservableProperty] private string _lastName = "";
    [ObservableProperty] private string _personalNumber = "";
    [ObservableProperty] private string _phone = "";
    [ObservableProperty] private string _street = "";
    [ObservableProperty] private string _postalCode = "";
    [ObservableProperty] private string _city = "";
    [ObservableProperty] private string _initialExpertise = "";

    partial void OnSelectedTeacherChanged(Teacher? value)
    {
        SelectedExpertise.Clear();
        if (value == null)
            return;
        foreach (var area in value.Expertise)
            SelectedExpertise.Add(area);
    }

    protected override Task LoadAsync() => RunAsync(async () =>
    {
        var teachers = await _db.GetAllAsync<Teacher>(Collections.Teachers);
        Teachers.Clear();
        foreach (var teacher in teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName))
            Teachers.Add(teacher);
        ShowInfo($"{Teachers.Count} lärare hämtade.");
    });

    [RelayCommand]
    private Task Refresh() => LoadAsync();

    [RelayCommand]
    private async Task AddTeacher()
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
        if (Teachers.Any(t => t.PersonalNumber == PersonalNumber.Trim()))
        {
            ShowError($"Personnummer {PersonalNumber.Trim()} finns redan registrerat.");
            return;
        }

        var teacher = new Teacher
        {
            FirstName = FirstName.Trim(),
            LastName = LastName.Trim(),
            PersonalNumber = PersonalNumber.Trim(),
            Phone = Phone.Trim(),
            Street = Street.Trim(),
            PostalCode = PostalCode.Trim(),
            City = City.Trim(),
            Expertise = InitialExpertise.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
        };

        await RunAsync(async () =>
        {
            var saved = await _db.AddAsync(Collections.Teachers, teacher);
            Teachers.Add(saved);
            FirstName = LastName = PersonalNumber = Phone = Street = PostalCode = City = InitialExpertise = "";
        }, $"Läraren {teacher.FullName} lades till.");
    }

    private bool CanAddExpertise() => SelectedTeacher != null && !string.IsNullOrWhiteSpace(NewExpertise);

    [RelayCommand(CanExecute = nameof(CanAddExpertise))]
    private async Task AddExpertise()
    {
        var area = NewExpertise.Trim();
        if (SelectedExpertise.Any(e => string.Equals(e, area, StringComparison.OrdinalIgnoreCase)))
        {
            ShowError($"{area} finns redan som kompetensområde.");
            return;
        }

        await SaveExpertiseAsync(SelectedExpertise.Append(area).ToList(), $"{area} lades till.");
        NewExpertise = "";
    }

    [RelayCommand]
    private Task RemoveExpertise(string area) =>
        SaveExpertiseAsync(SelectedExpertise.Where(e => e != area).ToList(), $"{area} togs bort.");

    /// <summary>Sparar lärarens nya lista med kompetensområden i RestDb och uppdaterar listan.</summary>
    private async Task SaveExpertiseAsync(List<string> expertise, string successMessage)
    {
        var teacher = SelectedTeacher;
        if (teacher?.Id == null)
            return;

        var updated = new Teacher
        {
            Id = teacher.Id,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            PersonalNumber = teacher.PersonalNumber,
            Phone = teacher.Phone,
            Street = teacher.Street,
            PostalCode = teacher.PostalCode,
            City = teacher.City,
            Expertise = expertise
        };

        await RunAsync(async () =>
        {
            await _db.UpdateAsync(Collections.Teachers, teacher.Id, updated);

            // Byt ut posten i listan så att vyn visar de nya kompetensområdena.
            var index = Teachers.IndexOf(teacher);
            if (index >= 0)
                Teachers[index] = updated;
            SelectedTeacher = updated;
        }, successMessage);
    }
}
