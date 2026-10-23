namespace WestcoastEducation.Models;

/// <summary>Lärare med kunskapsområden och ansvar för kurser.</summary>
public class Teacher : Person
{
    public List<string> Expertise { get; } = new();

    // AGGREGATION: kurserna existerar oberoende av läraren.
    public List<Course> Courses { get; } = new();

    public Teacher(string firstName, string lastName, string phone, string personalNumber,
        string street, string postalCode, string city, IEnumerable<string>? expertise = null)
        : base(firstName, lastName, phone, personalNumber, street, postalCode, city)
    {
        if (expertise != null)
            Expertise.AddRange(expertise.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e.Trim()));
    }

    /// <summary>Rollnamn som visas i utskrifter. Subklasser överskuggar detta.</summary>
    protected virtual string RoleName => "Lärare";

    /// <summary>Lägger till en kurs och håller associationen i synk åt båda håll.</summary>
    public void AddCourse(Course course)
    {
        if (Courses.Contains(course))
            return;

        Courses.Add(course);
        course.AssignTeacher(this); // synkar andra riktningen
    }

    public override string ToString() =>
        $"{RoleName}: {base.ToString()}, kunskapsområden: {(Expertise.Count > 0 ? string.Join(", ", Expertise) : "inga")}, " +
        $"kurser: {Courses.Count}";
}
