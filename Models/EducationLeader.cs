namespace WestcoastEducation.Models;

/// <summary>Utbildningsledare – en lärare med anställningsdatum.</summary>
public class EducationLeader : Teacher
{
    public DateTime HireDate { get; }

    public EducationLeader(string firstName, string lastName, string phone, string personalNumber,
        string street, string postalCode, string city, DateTime hireDate, IEnumerable<string>? expertise = null)
        : base(firstName, lastName, phone, personalNumber, street, postalCode, city, expertise)
    {
        HireDate = hireDate;
    }

    protected override string RoleName => "Utbildningsledare";

    public override string ToString() => $"{base.ToString()}, anställd {HireDate:yyyy-MM-dd}";
}
