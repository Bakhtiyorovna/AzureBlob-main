using System.Linq.Expressions;

namespace AzureBlob1.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Expression<Func<T, bool>> expression);
    Task<T> SelectAsync(Expression<Func<T, bool>> expression, string[] includes = null);
    IQueryable<T> SelectAll(Expression<Func<T, bool>> expression = null, string[] includes = null, bool isTracking = false);
    Task SaveAsync();
}