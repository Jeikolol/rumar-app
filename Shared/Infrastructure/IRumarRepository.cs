using Shared.Application.Common.Interfaces;
using Shared.Application.Common.Models;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure
{
    public interface IRumarRepository<T>
     where T : class, IEntity
    {
        IQueryable<T> Table();
        IQueryable<T> TableForUpdate();

        void DisableFilterAggregator();
        void EnableFilterAggregator();

        void Insert(T entity);
        void InsertRange(IEnumerable<T> entities);
        Task InsertRangeAndSaveAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
        Task<T> InsertAndSaveAsync(T entity, CancellationToken cancellationToken);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        Task UpdateRangeAndSaveAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
        Task UpdateAndSaveAsync(T entity, CancellationToken cancellationToken);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task DeleteAndSaveAsync(T entity, CancellationToken cancellationToken);
        Task DeleteRangeAndSaveAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
            where TId : notnull;

        Task<T?> GetWithIncludeByIdAsync<TId>(TId id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
            where TId : notnull;

        Task<T?> GetWithConditionsByIdAsync<TId>(
            TId id,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
            where TId : notnull;

        Task<T?> GetWithIncludeAndConditionsByIdAsync<TId>(
            TId id,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
            where TId : notnull;

        IQueryable<T> TablePaginated(PaginationFilter filter, Expression<Func<T, bool>>? predicate = null);

        Task<long> GenerateNextAtomicSequenceAsync<TEntity>()
            where TEntity : IEntity, IHaveAtomicSequence;

        Task<long> GenerateNextAtomicSequenceAsync<TEntity>(Guid accountingTransactionTypeId)
            where TEntity : IEntity, IHaveAtomicSequence;

        Task<IEnumerable<T>> Filter<TFilter>(decimal amount, LogicOperator opt, string schema, string columnToFilter)
            where TFilter : class, IEntity;

        Task SoftDeleteSaveAsync<TSoftDelete>(TSoftDelete entity, SoftDeleteParameter softDeleteParameter, CancellationToken cancellationToken)
            where TSoftDelete : T, ISoftDelete;

        void SoftDelete<TSoftDelete>(TSoftDelete entity, SoftDeleteParameter softDeleteParameter)
            where TSoftDelete : T, ISoftDelete;

        Task<bool> UspEntityHasDependencies(string schema, string tableName, Guid id);
        IEnumerable<TEntity> SetUserTimeZoneToDatePropertiesCollection<TEntity, T2>(List<TEntity> listToUpdate, params Expression<Func<TEntity, T2>>[] propertiesToUtc);
    }

}
