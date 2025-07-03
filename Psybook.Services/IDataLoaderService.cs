namespace Application.Common.Interfaces;
public interface IDataLoaderService<T>
{
    Task<HashSet<T>> LoadData();
}