﻿using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace Birder.Data.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext DbContext;
    public Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<TEntity> GetAsync(int id)
    {
        // Here we are working with a DbContext, not PlutoContext. So we don't have DbSets 
        // such as Courses or Authors, and we need to use the generic Set() method to access them.
        //return Context.Set<TEntity>().Find(id);
        return await DbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        // Note that here I've repeated Context.Set<TEntity>() in every method and this is causing
        // too much noise. I could get a reference to the DbSet returned from this method in the 
        // constructor and store it in a private field like _entities. This way, the implementation
        // of our methods would be cleaner:
        // 
        // _entities.ToList();
        // _entities.Where();
        // _entities.SingleOrDefault();
        // 
        // I didn't change it because I wanted the code to look like the videos. But feel free to change
        // this on your own.
        //return Context.Set<TEntity>().ToList();
        return await DbContext.Set<TEntity>().ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbContext.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    public void Add(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().AddRange(entities);
    }

    public void Remove(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().RemoveRange(entities);
    }
}