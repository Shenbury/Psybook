using System.Net.Http.Json;
using Application.Common.Interfaces;

namespace Application.Features.CustomTable;

public class CustomDataLoaderService<T>(HttpClient _httpClient) : IDataLoaderService<T>
{
    public async Task<HashSet<T>> LoadData()
    {
        // Use EF or API to fetch
        var items = await _httpClient.GetFromJsonAsync<HashSet<T>>("yourURL");
        return items ?? [];
    }
}