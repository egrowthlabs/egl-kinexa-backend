using System.Linq.Expressions;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Common;
using EGL.Kinexa.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EGL.Kinexa.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly KinexaDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(KinexaDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Queryable => _dbSet;

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbSet.Where(filter).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void SoftDelete(T entity)
    {
        entity.IsDeleted = true;
        entity.DateDeleted = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
    }
}
