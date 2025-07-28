using Ardalis.Specification.EntityFrameworkCore;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Common.Interfaces;
using Shared.Application.Common.Models;
using Shared.Infrastructure.RepositoryQueryFilters;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure
{
    public class ErpRepository<T> : IRumarRepository<T>
    where T : class, IEntity
    {
        private readonly RumarRepositoryQueryAggregator _filterAggregator;
        private bool _isFilterAggretagorEnabled = true;

        public ApplicationDbContext dbContext { get; }

        public ErpRepository(ApplicationDbContext dbContext, RumarRepositoryQueryAggregator filterAggregator)
        {
            this.dbContext = dbContext;
            _filterAggregator = filterAggregator;
        }

        public IQueryable<T> Table()
        {
            var queryable = dbContext.Table<T>();

            if (_isFilterAggretagorEnabled)
            {
                queryable = _filterAggregator.AddRepositoryQueryFilters(queryable);
            }

            return queryable;
        }

        public IQueryable<T> TableForUpdate()
        {
            var queryable = dbContext.TableForUpdate<T>();

            if (_isFilterAggretagorEnabled)
            {
                queryable = _filterAggregator.AddRepositoryQueryFilters(queryable);
            }

            return queryable;
        }

        public void Insert(T entity)
        {
            dbContext.Set<T>().Add(entity);
        }

        public void InsertRange(IEnumerable<T> entities)
        {
            dbContext.Set<T>().AddRange(entities);
        }

        public async Task InsertRangeAndSaveAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            dbContext.Set<T>().AddRange(entities);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<T> InsertAndSaveAsync(T entity, CancellationToken cancellationToken)
        {
            Insert(entity);
            await SaveChangesAsync(cancellationToken);

            return entity;
        }

        public void Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            dbContext.Set<T>().UpdateRange(entities);
        }

        public async Task UpdateAndSaveAsync(T entity, CancellationToken cancellationToken)
        {
            Update(entity);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRangeAndSaveAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            UpdateRange(entities);
            await SaveChangesAsync(cancellationToken);
            await SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// This method set SoftDelete properties in BaseDbContext.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        /// <summary>
        /// This method set SoftDelete properties in BaseDbContext.
        /// </summary>
        /// <param name="entities"></param>
        public void DeleteRange(IEnumerable<T> entities)
        {
            dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task DeleteAndSaveAsync(T entity, CancellationToken cancellationToken)
        {
            Delete(entity);
            await SaveChangesAsync(cancellationToken);
        }

        public void SoftDelete<TSoftDelete>(TSoftDelete entity, SoftDeleteParameter softDeleteParameter)
            where TSoftDelete : T, ISoftDelete
        {
            // if (string.IsNullOrWhiteSpace(entity.DeletedReason))
            //    throw new LinquerpException("delete reason is empty.");

            entity.DeletedOn = DateTime.UtcNow;
            entity.DeletedBy = dbContext.CurrentUser.GetUserId();

            Update(entity);
        }

        public async Task SoftDeleteSaveAsync<TSoftDelete>(TSoftDelete entity, SoftDeleteParameter softDeleteParameter, CancellationToken cancellationToken)
            where TSoftDelete : T, ISoftDelete
        {
            // if (string.IsNullOrWhiteSpace(entity.DeletedReason))
            //    throw new LinquerpException("delete reason is empty.");

            entity.DeletedOn = DateTime.UtcNow;
            entity.DeletedBy = dbContext.CurrentUser.GetUserId();

            Update(entity);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRangeAndSaveAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            DeleteRange(entities);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
            where TId : notnull
        {
            return await dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        }

        public async Task<T?> GetWithIncludeByIdAsync<TId>(TId id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
        where TId : notnull
        {
            var query = dbContext.Set<T>().AsQueryable();

            foreach (var include in includes)
            {
                if (include != null)
                {
                    query = query.Include(include);
                }
            }

            return await query
                .FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id), cancellationToken);
        }

        public async Task<T?> GetWithConditionsByIdAsync<TId>(
            TId id,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
            where TId : notnull
        {
            IQueryable<T> query = dbContext.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query
                .FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id), cancellationToken);
        }

        public async Task<T?> GetWithIncludeAndConditionsByIdAsync<TId>(
            TId id,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
            where TId : notnull
        {
            var query = dbContext.Set<T>().AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var include in includes)
            {
                if (include != null)
                {
                    query = query.Include(include);
                }
            }

            return await query
                .FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id), cancellationToken);
        }

        public IQueryable<T> TablePaginated(PaginationFilter filter, Expression<Func<T, bool>>? predicate = null)
        {
            filter.PageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            IQueryable<T> query = predicate is not null ?
                ApplyFilter(Table().Where(predicate), filter) :
                ApplyFilter(Table(), filter);

            return query.Take(filter.PageSize);
        }

        public async Task<bool> UspEntityHasDependencies(string schema, string tableName, Guid id)
        {
            return await dbContext.EntityHasDependencies(schema, tableName, id);
        }

        public async Task<long> GenerateNextAtomicSequenceAsync<TEntity>()
            where TEntity : IEntity, IHaveAtomicSequence
        {
            return await dbContext.GenerateNextAtomicSequenceAsync<TEntity>();
        }

        public async Task<long> GenerateNextAtomicSequenceAsync<TEntity>(Guid accountingTransactionTypeId)
            where TEntity : IEntity, IHaveAtomicSequence
        {
            return await dbContext.GenerateNextAtomicSequenceAsync<TEntity>(accountingTransactionTypeId);
        }

        public async Task<IEnumerable<T>> Filter<TFilter>(decimal amount, LogicOperator opt, string schema, string columnToFilter)
            where TFilter : class, IEntity
        {
            return await dbContext.Filter<T>(amount, opt, schema, columnToFilter);
        }

        public void DisableFilterAggregator()
        {
            _isFilterAggretagorEnabled = false;
        }

        public void EnableFilterAggregator()
        {
            _isFilterAggretagorEnabled = true;
        }

        public IEnumerable<TEntity> SetUserTimeZoneToDatePropertiesCollection<TEntity, T2>(List<TEntity> listToUpdate, params Expression<Func<TEntity, T2>>[] propertiesToUtc)
        {
            listToUpdate.ForEach(x =>
            {
                foreach (var p in propertiesToUtc)
                {
                    var valueExpression = (MemberExpression)p.Body;
                    string? dataValueField = valueExpression.Member.Name;

                    var propertySource = typeof(T).GetProperties().First(proper => proper.Name == dataValueField);

                    if (propertySource == null) return;

                    var propertyTargetInfo = x?.GetType().GetProperty(propertySource.Name);

                    if (propertyTargetInfo == null) return;

                    var propertyValue = (DateTime?)propertyTargetInfo.GetValue(x);

                    if (propertyValue == null) return;

                    var utcDate = propertyValue.Value.UtcToLocal();
                    propertyTargetInfo.SetValue(x, utcDate);
                }
            });

            return listToUpdate;
        }

        private IQueryable<T> ApplyFilter(IQueryable<T> query, PaginationFilter filter)
        {
            query = ApplyKeywordFilter(query, filter);
            query = ApplyAdvancedSearchFilter(query, filter);
            query = ApplyOrderByFilter(query, filter.OrderBy, true);
            query = ApplyOrderByFilter(query, filter.OrderByDesc, false);
            query = ApplyLastIdFilter(query, filter);
            query = ApplyPageNumberFilter(query, filter);

            return query;
        }

        private IQueryable<T> ApplyKeywordFilter(IQueryable<T> query, PaginationFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                var spec = new EntitiesByBaseFilterSpec<T>(filter);
                query = query.WithSpecification(spec);
            }

            return query;
        }

        private IQueryable<T> ApplyAdvancedSearchFilter(IQueryable<T> query, PaginationFilter filter)
        {
            if (filter.AdvancedSearch != null && !string.IsNullOrEmpty(filter.AdvancedSearch.Keyword))
            {
                var spec = new EntitiesByBaseFilterSpec<T>(filter);
                query = query.WithSpecification(spec);
            }

            return query;
        }

        private IQueryable<T> ApplyOrderByFilter(IQueryable<T> query, string[]? orderBy, bool ascending)
        {
            if (orderBy?.Length > 0)
            {
                foreach (string propertyName in orderBy)
                {
                    var prop = typeof(T).GetProperty(propertyName);

                    if (prop is null)
                    {
                        continue;
                    }

                    var parameter = Expression.Parameter(typeof(T));
                    var property = Expression.Property(parameter, propertyName.ToLower());
                    var propAsObject = Expression.Convert(property, typeof(object));

                    query = ascending ? query.OrderBy(Expression.Lambda<Func<T, object>>(propAsObject, parameter))
                                      : query.OrderByDescending(Expression.Lambda<Func<T, object>>(propAsObject, parameter));
                }
            }

            return query;
        }

        private IQueryable<T> ApplyLastIdFilter(IQueryable<T> query, PaginationFilter filter)
        {
            if (filter.LastId != Guid.Empty)
            {
                var prop = typeof(T).GetProperty("Id");

                if (prop is not null)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var member = Expression.Property(parameter, "Id");
                    var expConstant = Expression.Constant(filter.LastId);
                    var expBody = Expression.GreaterThan(member, expConstant);

                    query = query.Where(Expression.Lambda<Func<T, bool>>(expBody, parameter));
                }
            }

            return query;
        }

        private IQueryable<T> ApplyPageNumberFilter(IQueryable<T> query, PaginationFilter filter)
        {
            if (filter.PageNumber > 1)
            {
                query = query.Skip((filter.PageNumber - 1) * filter.PageSize);
            }

            return query;
        }
    }
}
