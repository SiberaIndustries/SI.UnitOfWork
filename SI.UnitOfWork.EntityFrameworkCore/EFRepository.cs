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
        public EFRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = DbContext.Set<TEntity>();
        }

        protected DbContext DbContext { get; }

        protected DbSet<TEntity> DbSet { get; }

        public virtual Task<TEntity> FindAsync(params object[]? keyValues) =>
            DbSet.FindAsync(keyValues, default).AsTask();

        public virtual Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, int pageIndex = 0, int pageSize = int.MaxValue, bool disableTracking = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>? orderBy = null, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = DbSet;
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
            ? DbSet.CountAsync(ct)
            : DbSet.CountAsync(predicate, ct);

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.LongCountAsync(ct)
            : DbSet.LongCountAsync(predicate, ct);

        public virtual Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, T>>? selector = null, CancellationToken ct = default) => predicate == null
            ? DbSet.MaxAsync(selector, ct)
            : DbSet.Where(predicate).MaxAsync(selector, ct);

        public virtual Task<T> MinAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, T>>? selector = null, CancellationToken ct = default) => predicate == null
            ? DbSet.MinAsync(selector, ct)
            : DbSet.Where(predicate).MinAsync(selector, ct);

        public virtual Task<decimal> AverageAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, decimal>>? selector = null, CancellationToken ct = default) => predicate == null
            ? DbSet.AverageAsync(selector, ct)
            : DbSet.Where(predicate).AverageAsync(selector, ct);

        public virtual Task<decimal> SumAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, decimal>>? selector = null, CancellationToken ct = default) => predicate == null
            ? DbSet.SumAsync(selector, ct)
            : DbSet.Where(predicate).SumAsync(selector, ct);

        public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null, CancellationToken ct = default) => selector == null
            ? DbSet.AnyAsync(ct)
            : DbSet.AnyAsync(selector, ct);

        public virtual Task InsertAsync(TEntity entity, CancellationToken ct = default) =>
            DbSet.AddAsync(entity, ct).AsTask();

        public virtual Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
            DbSet.AddRangeAsync(entities, ct);

        public virtual Task UpdateAsync(TEntity entity, CancellationToken ct = default) =>
            Task.FromResult(DbSet.Update(entity));

        public virtual Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            DbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(object id, CancellationToken ct = default)
        {
            // using a stub entity to mark for deletion
            var typeInfo = typeof(TEntity).GetTypeInfo();
            var key = DbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties[0];
            var property = typeInfo.GetProperty(key?.Name);
            if (property != null)
            {
                var entity = Activator.CreateInstance<TEntity>();
                property.SetValue(entity, id);
                DbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                var entity = DbSet.Find(id);
                if (entity != null)
                {
                    return DeleteAsync(entity, ct);
                }
            }

            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken ct = default) =>
            Task.FromResult(DbSet.Remove(entity));

        public virtual Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
        {
            DbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}
