using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork
{
    public class EFRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext dbContext;
        protected readonly DbSet<TEntity> dbSet;

        public EFRepository(DbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            dbSet = this.dbContext.Set<TEntity>();
        }

        public virtual Task<TEntity> FindAsync(params object[]? keyValues) =>
            dbSet.FindAsync(keyValues, default).AsTask();

        public virtual Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, int pageIndex = 0, int pageSize = int.MaxValue, bool disableTracking = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>? orderBy = null, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            return Task.FromResult(query.AsEnumerable());
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? dbSet.CountAsync(ct)
            : dbSet.CountAsync(predicate, ct);

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? dbSet.LongCountAsync(ct)
            : dbSet.LongCountAsync(predicate, ct);

        public virtual Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, T>>? selector = null, CancellationToken ct = default) => predicate == null
            ? dbSet.MaxAsync(selector, ct)
            : dbSet.Where(predicate).MaxAsync(selector, ct);

        public virtual Task<T> MinAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, T>>? selector = null, CancellationToken ct = default) => predicate == null
            ? dbSet.MinAsync(selector, ct)
            : dbSet.Where(predicate).MinAsync(selector, ct);

        public virtual Task<decimal> AverageAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, decimal>>? selector = null, CancellationToken ct = default) => predicate == null
            ? dbSet.AverageAsync(selector, ct)
            : dbSet.Where(predicate).AverageAsync(selector, ct);

        public virtual Task<decimal> SumAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, decimal>>? selector = null, CancellationToken ct = default) => predicate == null
            ? dbSet.SumAsync(selector, ct)
            : dbSet.Where(predicate).SumAsync(selector, ct);

        public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null, CancellationToken ct = default) => selector == null
            ? dbSet.AnyAsync(ct)
            : dbSet.AnyAsync(selector, ct);

        public virtual Task InsertAsync(TEntity entity, CancellationToken ct = default) =>
            dbSet.AddAsync(entity, ct).AsTask();

        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
            dbSet.AddRangeAsync(entities, ct);

        public virtual Task UpdateAsync(TEntity entity, CancellationToken ct = default) =>
            Task.FromResult(dbSet.Update(entity));

        public virtual Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(object id, CancellationToken ct = default)
        {
            // using a stub entity to mark for deletion
            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = dbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
            var property = typeInfo.GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                dbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = dbSet.Find(id);
                if (entity != null)
                {
                    return DeleteAsync(entity, ct);
                }
            }

            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken ct = default) =>
            Task.FromResult(dbSet.Remove(entity));

        public virtual Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}
