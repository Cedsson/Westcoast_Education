namespace WestcoastEducation.Models;

/// <summary>Administratör – har samma information som en utbildningsledare.</summary>
public class Administrator : EducationLeader
{
    public Administrator(string firstName, string lastName, string phone, string personalNumber,
        string street, string postalCode, string city, DateTime hireDate, IEnumerable<string>? expertise = null)
        : base(firstName, lastName, phone, personalNumber, street, postalCode, city, hireDate, expertise)
    {
    }

    protected override string RoleName => "Administratör";
}
