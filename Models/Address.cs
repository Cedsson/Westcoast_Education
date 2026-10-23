namespace WestcoastEducation.Models;

/// <summary>
/// Adress för en person.
/// KOMPOSITION: En Address skapas inuti Person och lever/dör med den.
/// Ingen Address existerar utan sin Person.
/// </summary>
public class Address
{
    public string Street { get; }
    public string PostalCode { get; }
    public string City { get; }

    public Address(string street, string postalCode, string city)
    {
        Street = street;
        PostalCode = postalCode;
        City = city;
    }

    public override string ToString() => $"{Street}, {PostalCode} {City}";
}
