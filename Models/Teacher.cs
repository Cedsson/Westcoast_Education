using System.Text.Json.Serialization;

namespace WestcoastEducation.Models;

//Lärare med kompetensområden.</summary>
public class Teacher : Person
{
    public List<string> Expertise { get; set; } = new();

    [JsonIgnore]
    public string ExpertiseText => Expertise.Count > 0 ? string.Join(", ", Expertise) : "Inga kompetensområden";
}
