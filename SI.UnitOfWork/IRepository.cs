﻿using System.Linq.Expressions;

namespace SI.UnitOfWork
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity?> FindAsync(object?[]? keyValues, CancellationToken ct = default);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, int pageIndex = 0, int pageSize = int.MaxValue, bool disableTracking = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>? orderBy = null, CancellationToken ct = default);

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        IAsyncEnumerable<TEntity> GetAllAsyncEnumerable(Expression<Func<TEntity, bool>>? predicate = null, int pageIndex = 0, int pageSize = int.MaxValue, bool disableTracking = true, bool ignoreQueryFilters = false, Expression<Func<TEntity, object>>? orderBy = null, CancellationToken ct = default);

#endif
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

        Task<TEntity> MaxAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
        
        Task<T> MaxAsync<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

        Task<TEntity> MinAsync<T>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
        
        Task<T> MinAsync<T>(Expression<Func<TEntity, T>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

        Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

        Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null, CancellationToken ct = default);

        Task InsertAsync(TEntity entity, CancellationToken ct = default);

        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

        Task UpdateAsync(TEntity entity, CancellationToken ct = default);

        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

        Task DeleteAsync(object id, CancellationToken ct = default);

        Task DeleteAsync(TEntity entity, CancellationToken ct = default);

        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    }
}
