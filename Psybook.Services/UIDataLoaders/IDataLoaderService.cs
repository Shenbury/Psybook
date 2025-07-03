namespace Psybook.Services.UIDataLoaders;
public interface IDataLoaderService<T>
{
    Task<HashSet<T>> GetMultiple(string url);
}