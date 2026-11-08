namespace WestcoastEducation.Services;

//Generisk datahantering mot RestDb.io-collections.</summary>
public interface IRestDbService
{
    Task<List<T>> GetAllAsync<T>(string collection);
    Task<T> AddAsync<T>(string collection, T item);
    Task<T> UpdateAsync<T>(string collection, string id, T item);
    Task DeleteAsync(string collection, string id);
}

/// <summary>Namn på collections i RestDb.io.</summary>
public static class Collections
{
    public const string Courses = "courses";
    public const string Students = "students";
    public const string Teachers = "teachers";
}
