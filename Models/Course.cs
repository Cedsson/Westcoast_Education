namespace WestcoastEducation.Models;

/// <summary>
/// En kurs. AGGREGATION: Course refererar till en Teacher och en lista Students
/// som existerar oberoende av kursen.
/// </summary>
public class Course
{
    public const int MinParticipants = 5;
    public const int CancellationWarningDays = 21; // 3 veckor

    public string CourseNumber { get; }
    public string Title { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public CourseType Type { get; }
    public Teacher? Teacher { get; private set; }
    public List<Student> Students { get; } = new();

    /// <summary>Längd i dagar (inklusive start- och slutdag).</summary>
    public int LengthInDays => (EndDate.Date - StartDate.Date).Days + 1;

    public Course(string courseNumber, string title, DateTime startDate, DateTime endDate, CourseType type)
    {
        if (string.IsNullOrWhiteSpace(courseNumber))
            throw new ArgumentException("Kursnummer får inte vara tomt.");
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Titel får inte vara tom.");
        if (endDate < startDate)
            throw new ArgumentException("Slutdatum får inte vara före startdatum.");

        CourseNumber = courseNumber.Trim();
        Title = title.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Type = type;
    }

    /// <summary>Lägger till en student, utan dubbletter, och synkar studentens kurslista.</summary>
    public void AddStudent(Student student)
    {
        if (Students.Any(s => s.PersonalNumber == student.PersonalNumber))
            throw new InvalidOperationException($"{student.FullName} är redan bokad på kursen.");

        Students.Add(student);
        student.BookedCourses.Add(this);
    }

    /// <summary>Tar bort en student via personnummer. Returnerar false om studenten inte fanns.</summary>
    public bool RemoveStudent(string personalNumber)
    {
        var student = Students.FirstOrDefault(s => s.PersonalNumber == personalNumber);
        if (student == null)
            return false;

        Students.Remove(student);
        student.BookedCourses.Remove(this);
        return true;
    }

    /// <summary>Sätter ansvarig lärare och håller lärarens kurslista i synk.</summary>
    public void AssignTeacher(Teacher teacher)
    {
        if (Teacher == teacher)
            return;

        Teacher?.Courses.Remove(this); // släpp tidigare lärare
        Teacher = teacher;
        teacher.AddCourse(this); // synkar andra riktningen
    }

    /// <summary>Kursen riskerar att ställas in om färre än 5 deltagare återstår 3 veckor före start.</summary>
    public bool IsAtRiskOfCancellation(DateTime today)
    {
        var daysToStart = (StartDate.Date - today.Date).Days;
        return daysToStart >= 0 && daysToStart <= CancellationWarningDays && Students.Count < MinParticipants;
    }

    public string TypeText => Type switch
    {
        CourseType.Klassrum => "Klassrum",
        CourseType.Distans => "Distans",
        _ => "Okänd"
    };

    public override string ToString() =>
        $"[{CourseNumber}] {Title} – {TypeText}, {StartDate:yyyy-MM-dd} till {EndDate:yyyy-MM-dd} " +
        $"({LengthInDays} dagar), lärare: {Teacher?.FullName ?? "ingen"}, deltagare: {Students.Count}";
}
