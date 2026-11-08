using System.Text.Json;

namespace WestcoastEducation.Services;

//Anslutningsuppgifter till RestDb.io, läses från appsettings.json.</summary>
public class RestDbSettings
{
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";

    private class SettingsFile
    {
        public RestDbSettings? RestDb { get; set; }
    }

    public static RestDbSettings Load(string fileName = "appsettings.json")
    {
        var path = Path.Combine(AppContext.BaseDirectory, fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException(
                "Hittade inte appsettings.json. Kopiera appsettings.example.json och fyll i RestDb-uppgifterna.", path);

        var file = JsonSerializer.Deserialize<SettingsFile>(File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var settings = file?.RestDb;
        if (settings == null || string.IsNullOrWhiteSpace(settings.BaseUrl) || string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("RestDb BaseUrl och ApiKey måste anges i appsettings.json.");

        return settings;
    }
}
