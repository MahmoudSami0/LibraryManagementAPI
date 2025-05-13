using System.Linq.Expressions;

namespace API_Structure.Core.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<IQueryable<T?>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> FindAsync(Expression<Func<T,bool>> criteria, string[]? includes = null);

    Task<TResult> CustomFindAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector, 
        Expression<Func<TEntity, bool>>? predicate = null, 
        Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<List<TResult>> CustomFindListAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<T?> AddAsync(T entity);
    Task<T?> UpdateAsync(T entity);
    Task<T?> DeleteAsync(T entity);
    Task<List<T>> DeleteRange(List<T> entities);
}