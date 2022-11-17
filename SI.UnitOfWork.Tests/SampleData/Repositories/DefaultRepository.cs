using SI.UnitOfWork.Interfaces;
using SI.UnitOfWork.Tests.SampleData.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SI.UnitOfWork.Tests.SampleData.Repositories
{
    public class DefaultRepository : IRepository<Person>
    {
        public DefaultRepository(IDbContext dbContext)
        {
            _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<decimal> AverageAsync(Expression<Func<Person, decimal>> selector, Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<int> CountAsync(Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task DeleteAsync(object id, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task DeleteAsync(Person entity, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task DeleteAsync(IEnumerable<Person> entities, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<bool> ExistsAsync(Expression<Func<Person, bool>> selector = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<Person> FindAsync(params object[] keyValues) =>
            throw new NotImplementedException();

        public Task<IEnumerable<Person>> GetAllAsync(Expression<Func<Person, bool>> predicate = null, int pageIndex = 0, int pageSize = int.MaxValue, bool disableTracking = true, bool ignoreQueryFilters = false, Expression<Func<Person, object>> orderBy = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task InsertAsync(Person entity, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task InsertAsync(IEnumerable<Person> entities, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<long> LongCountAsync(Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<Person> MaxAsync<T>(Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<T> MaxAsync<T>(Expression<Func<Person, T>> selector, Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<Person> MinAsync<T>(Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<T> MinAsync<T>(Expression<Func<Person, T>> selector, Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task<decimal> SumAsync(Expression<Func<Person, decimal>> selector, Expression<Func<Person, bool>> predicate = null, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task UpdateAsync(Person entity, CancellationToken ct = default) =>
            throw new NotImplementedException();

        public Task UpdateAsync(IEnumerable<Person> entities, CancellationToken ct = default) =>
            throw new NotImplementedException();
    }
}
