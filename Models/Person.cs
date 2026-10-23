namespace WestcoastEducation.Models;

/// <summary>Abstrakt basklass med gemensam data för alla personer.</summary>
public abstract class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Phone { get; }
    public string PersonalNumber { get; }

    // KOMPOSITION: adressen skapas här inne av Person och delas inte med andra.
    public Address Address { get; }

    protected Person(string firstName, string lastName, string phone, string personalNumber,
        string street, string postalCode, string city)
    {
        if (string.IsNullOrWhiteSpace(personalNumber))
            throw new ArgumentException("Personnummer får inte vara tomt.");
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Förnamn och efternamn måste anges.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone.Trim();
        PersonalNumber = personalNumber.Trim();
        Address = new Address(street.Trim(), postalCode.Trim(), city.Trim());
    }

    public string FullName => $"{FirstName} {LastName}";

    public override string ToString() =>
        $"{FullName} (personnr {PersonalNumber}, tel {Phone}, adress {Address})";
}
