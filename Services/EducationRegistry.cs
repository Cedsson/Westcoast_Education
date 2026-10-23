using WestcoastEducation.Models;

namespace WestcoastEducation.Services;

/// <summary>Register över kurser, studerande, lärare, utbildningsledare och administratörer.</summary>
public class EducationRegistry
{
    public List<Course> Courses { get; } = new();
    public List<Student> Students { get; } = new();
    public List<Teacher> Teachers { get; } = new();
    public List<EducationLeader> EducationLeaders { get; } = new();
    public List<Administrator> Administrators { get; } = new();

    private IEnumerable<Person> AllPeople =>
        Students.Cast<Person>().Concat(AllTeachers);

    // Utbildningsledare och administratörer är också lärare tack vare arvet.
    private IEnumerable<Teacher> AllTeachers =>
        Teachers.Concat(EducationLeaders).Concat(Administrators);

    public void AddCourse(Course course)
    {
        if (Courses.Any(c => c.CourseNumber == course.CourseNumber))
            throw new InvalidOperationException($"Kursnummer {course.CourseNumber} finns redan.");
        Courses.Add(course);
    }

    public void AddStudent(Student student) => AddPerson(Students, student);
    public void AddTeacher(Teacher teacher) => AddPerson(Teachers, teacher);
    public void AddEducationLeader(EducationLeader leader) => AddPerson(EducationLeaders, leader);
    public void AddAdministrator(Administrator admin) => AddPerson(Administrators, admin);

    // Gemensam hjälpmetod: personnummer måste vara unikt över alla personer.
    private void AddPerson<T>(List<T> list, T person) where T : Person
    {
        if (AllPeople.Any(p => p.PersonalNumber == person.PersonalNumber))
            throw new InvalidOperationException($"Personnummer {person.PersonalNumber} finns redan registrerat.");
        list.Add(person);
    }

    public Course? FindCourse(string courseNumber) =>
        Courses.FirstOrDefault(c => c.CourseNumber == courseNumber.Trim());

    public Student? FindStudent(string personalNumber) =>
        Students.FirstOrDefault(s => s.PersonalNumber == personalNumber.Trim());

    public Teacher? FindTeacher(string personalNumber) =>
        AllTeachers.FirstOrDefault(t => t.PersonalNumber == personalNumber.Trim());

    /// <summary>Bokar en student på en kurs. Kastar fel om kurs eller student saknas.</summary>
    public void BookStudent(string courseNumber, string personalNumber)
    {
        var course = FindCourse(courseNumber)
            ?? throw new InvalidOperationException($"Kurs {courseNumber} hittades inte.");
        var student = FindStudent(personalNumber)
            ?? throw new InvalidOperationException($"Student med personnummer {personalNumber} hittades inte.");
        course.AddStudent(student);
    }

    /// <summary>Listar kurser, valfritt filtrerat på kurstyp.</summary>
    public void ListCourses(CourseType? filter = null)
    {
        var courses = filter == null ? Courses : Courses.Where(c => c.Type == filter).ToList();
        PrintList(courses, "Inga kurser registrerade");
    }

    public void ListStudents() => PrintList(Students, "Inga studerande registrerade");

    public void ListTeachers() => PrintList(AllTeachers.ToList(), "Inga lärare registrerade");

    /// <summary>Listar kurser som riskerar att ställas in (färre än 5 deltagare 3 veckor före start).</summary>
    public void ListCoursesAtRisk(DateTime today)
    {
        var risky = Courses.Where(c => c.IsAtRiskOfCancellation(today)).ToList();
        PrintList(risky, "Inga kurser riskerar att ställas in");
    }

    private static void PrintList<T>(IReadOnlyCollection<T> items, string emptyMessage)
    {
        if (items.Count == 0)
        {
            Console.WriteLine(emptyMessage);
            return;
        }

        foreach (var item in items)
            Console.WriteLine(item);
    }
}
