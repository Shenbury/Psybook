namespace Psybook.Services.UI.DataLoaders;
public interface IDataLoaderService<T>
{
    Task<List<T>> GetMultiple(string url);
}