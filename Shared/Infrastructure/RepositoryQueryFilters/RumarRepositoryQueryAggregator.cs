using Microsoft.Extensions.DependencyInjection;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.RepositoryQueryFilters
{
    public class RumarRepositoryQueryAggregator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<Type> _filterClasses;
        private List<FilterClassInterfacePair> _filterInterfaces = new();

        public RumarRepositoryQueryAggregator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _filterClasses = typeof(IRumarRepositoryQueryFilter<>).Assembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract &&
                                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRumarRepositoryQueryFilter<>)))
                    .ToList();

            foreach (var filterClass in _filterClasses)
            {
                var queryFilterGenericType = filterClass.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRumarRepositoryQueryFilter<>));
                var parameterType = queryFilterGenericType.GetGenericArguments()[0];

                var filterWithInterface = new FilterClassInterfacePair
                {
                    Interface = parameterType,
                    Class = filterClass,
                };

                _filterInterfaces.Add(filterWithInterface);
            }
        }

        public IQueryable<TEntity> AddRepositoryQueryFilters<TEntity>(IQueryable<TEntity> query)
            where TEntity : class, IEntity
        {
            Type[] entityInterfaces = typeof(TEntity).GetInterfaces();
            var filterInterfaces = entityInterfaces
                .Where(i => _filterInterfaces.Any(fi => fi.Interface == i))
                .ToList();

            if (filterInterfaces.Count == 0)
            {
                return query;
            }

            IQueryable<TEntity> resultQuery = query;

            foreach (var filterInterface in filterInterfaces)
            {
                var classFilter = _filterInterfaces.FirstOrDefault(fi => fi.Interface == filterInterface);

                if (classFilter == null)
                {
                    continue;
                }

                var serviceType = typeof(IRumarRepositoryQueryFilter<>).MakeGenericType(classFilter.Interface);
                var serviceFilter = _serviceProvider.GetRequiredService(serviceType);

                var canApplyMethod = serviceType.GetMethod(nameof(IRumarRepositoryQueryFilter<object>.CanApplyQuery));
                bool canApply = canApplyMethod?.Invoke(serviceFilter, null) as bool?
                    ?? false;

                if (canApply)
                {
                    var addQueryMethod = classFilter.Class
                        .GetMethod(nameof(IRumarRepositoryQueryFilter<object>.AddQueryFilter))?
                        .MakeGenericMethod(typeof(TEntity));

                    resultQuery = addQueryMethod?.Invoke(serviceFilter, new object[] { resultQuery }) as IQueryable<TEntity>
                        ?? resultQuery;
                }
            }

            return resultQuery;
        }

        private class FilterClassInterfacePair
        {
            public Type Interface { get; set; }
            public Type Class { get; set; }
        }
    }
}
