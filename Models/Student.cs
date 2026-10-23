namespace WestcoastEducation.Models;

/// <summary>Studerande som kan vara bokad på flera kurser.</summary>
public class Student : Person
{
    // AGGREGATION: kurserna existerar oberoende av studenten.
    public List<Course> BookedCourses { get; } = new();

    public Student(string firstName, string lastName, string phone, string personalNumber,
        string street, string postalCode, string city)
        : base(firstName, lastName, phone, personalNumber, street, postalCode, city)
    {
    }

    public override string ToString() => $"Student: {base.ToString()}, bokade kurser: {BookedCourses.Count}";
}
