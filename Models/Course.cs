using System.Text.Json.Serialization;

namespace WestcoastEducation.Models;

//En kurs som erbjuds av WestCoast Education.</summary>
public class Course
{
    //RestDb.io:s unika id för posten.</summary>
    [JsonPropertyName("_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }

    public string CourseNumber { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CourseType Type { get; set; }

    //Längd i dagar (inklusive start- och slutdag).</summary>
    [JsonIgnore]
    public int LengthInDays => (EndDate.Date - StartDate.Date).Days + 1;

    [JsonIgnore]
    public string DateText => $"{StartDate:yyyy-MM-dd} – {EndDate:yyyy-MM-dd} ({LengthInDays} dagar)";
}
