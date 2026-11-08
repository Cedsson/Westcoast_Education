using System.Text.Json.Serialization;

namespace WestcoastEducation.Models;

//Abstrakt basklass med gemensam data för alla personer.</summary>
public abstract class Person
{
    //RestDb.io:s unika id för posten.</summary>
    [JsonPropertyName("_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PersonalNumber { get; set; } = "";
    public string Street { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string City { get; set; } = "";

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    [JsonIgnore]
    public string AddressText => $"{Street}, {PostalCode} {City}";
}
