using System.Net.Http.Json;

namespace Psybook.Services.UI.DataLoaders;

public class CustomDataLoaderService<T>(HttpClient _httpClient) : IDataLoaderService<T>
{
    public async Task<List<T>> GetMultiple(string url)
    {
        // Use EF or API to fetch
        var items = await _httpClient.GetFromJsonAsync<List<T>>(url);
        return items ?? [];
    }
}