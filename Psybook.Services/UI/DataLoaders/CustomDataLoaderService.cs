using System.Net.Http.Json;

namespace Psybook.Services.UI.DataLoaders;

public class CustomDataLoaderService<T>(HttpClient _httpClient) : IDataLoaderService<T>
{
    public async Task<HashSet<T>> GetMultiple(string url)
    {
        // Use EF or API to fetch
        var items = await _httpClient.GetFromJsonAsync<HashSet<T>>(url);
        return items ?? [];
    }
}