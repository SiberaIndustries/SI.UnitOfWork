using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SI.UnitOfWork
{
    public class EFRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public EFRepository(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
        }

        protected DbContext DbContext { get; }

        protected DbSet<TEntity> DbSet { get; }

        public virtual Task<TEntity?> FindAsync(params object[]? keyValues) =>
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

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        public IAsyncEnumerable<TEntity> GetAllAsyncEnumerable(Expression<Func<TEntity, bool>>? predicate = null, int pageIndex = 0, int pageSize = int.MaxValue, bool disableTracking = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>? orderBy = null, CancellationToken ct = default)
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
            return query.AsAsyncEnumerable();
        }

#endif
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.CountAsync(ct)
            : DbSet.CountAsync(predicate, ct);

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.LongCountAsync(ct)
            : DbSet.LongCountAsync(predicate, ct);

        public virtual Task<TEntity> MaxAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.MaxAsync(ct)
            : DbSet.Where(predicate).MaxAsync(ct);

        public virtual Task<T> MaxAsync<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.MaxAsync(selector, ct)
            : DbSet.Where(predicate).MaxAsync(selector, ct);

        public virtual Task<TEntity> MinAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.MinAsync(ct)
            : DbSet.Where(predicate).MinAsync(ct);

        public virtual Task<T> MinAsync<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.MinAsync(selector, ct)
            : DbSet.Where(predicate).MinAsync(selector, ct);

        public virtual Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
            ? DbSet.AverageAsync(selector, ct)
            : DbSet.Where(predicate).AverageAsync(selector, ct);

        public virtual Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default) => predicate == null
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

        public virtual async Task DeleteAsync(object id, CancellationToken ct = default)
        {
            var entity = await FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                await DeleteAsync(entity, ct).ConfigureAwait(false);
            }
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
