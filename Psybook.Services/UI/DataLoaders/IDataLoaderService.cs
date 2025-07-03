namespace Psybook.Services.UI.DataLoaders;
public interface IDataLoaderService<T>
{
    Task<HashSet<T>> GetMultiple(string url);
}