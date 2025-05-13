using API_Structure.Core.Repositories;
using API_Structure.EF.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API_Structure.EF.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<T?>> GetAllAsync()
    {
        return await Task.Run(() => _context.Set<T>().AsQueryable());
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> criteria, string[]? includes = null)
    {
        IQueryable<T> query = _context.Set<T>();

        if (query is not null)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }
        }

        return await query.SingleOrDefaultAsync(criteria);
    }

    public async Task<TResult> CustomFindAsync<TEntity, TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
    CancellationToken cancellationToken = default)
    where TEntity : class
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (includes != null)
        {
            query = includes(query);
        }

        if (predicate != null)
            return await query
            .Where(predicate)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);

        return await query
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<List<TResult>> CustomFindListAsync<TEntity, TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (includes != null)
        {
            query = includes(query);
        }

        if (predicate != null)
            return await _context.Set<TEntity>()
            .Where(predicate)
            .Select(selector)
            .ToListAsync(cancellationToken);
        
        return await _context.Set<TEntity>()
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<T?> UpdateAsync(T entity)
    {
        await Task.Run(() =>
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        });
        return entity;
    }

    public async Task<T?> DeleteAsync(T entity)
    {
        await Task.Run(() =>
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        });
        return entity;
    }

    public async Task<List<T>> DeleteRange(List<T> entities)
    {
        await Task.Run(() =>
        {
            _context.Set<T>().RemoveRange(entities);
            _context.SaveChanges();
        });
        return entities;
    }
}