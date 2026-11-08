using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WestcoastEducation.Services;

/// <summary>Pratar med RestDb.io:s REST-API: {BaseUrl}/rest/{collection}.</summary>
public class RestDbService : IRestDbService
{
    // Klienten skapas först vid första anropet, så att ett konfigurationsfel
    // visas som ett felmeddelande i appen i stället för att appen kraschar vid start.
    private readonly Lazy<HttpClient> _client;
    private HttpClient _http => _client.Value;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public RestDbService(Func<RestDbSettings> settingsFactory)
    {
        _client = new Lazy<HttpClient>(() =>
        {
            var settings = settingsFactory();
            var http = new HttpClient { BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/rest/") };
            http.DefaultRequestHeaders.Add("x-apikey", settings.ApiKey);
            http.DefaultRequestHeaders.Add("cache-control", "no-cache");
            return http;
        }, LazyThreadSafetyMode.PublicationOnly);
    }

    public async Task<List<T>> GetAllAsync<T>(string collection)
    {
        using var response = await _http.GetAsync(collection);
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<List<T>>(JsonOptions) ?? new List<T>();
    }

    public async Task<T> AddAsync<T>(string collection, T item)
    {
        using var response = await _http.PostAsJsonAsync(collection, item, JsonOptions);
        await EnsureSuccess(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;
    }

    public async Task<T> UpdateAsync<T>(string collection, string id, T item)
    {
        using var response = await _http.PutAsJsonAsync($"{collection}/{id}", item, JsonOptions);
        await EnsureSuccess(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;
    }

    public async Task DeleteAsync(string collection, string id)
    {
        using var response = await _http.DeleteAsync($"{collection}/{id}");
        await EnsureSuccess(response);
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"RestDb svarade {(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }
}
