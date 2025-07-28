using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.RepositoryQueryFilters
{
    public interface IRumarRepositoryQueryFilter<T>
    {
        public abstract bool CanApplyQuery();
        public abstract IQueryable<TEntity> AddQueryFilter<TEntity>(IQueryable<TEntity> query)
            where TEntity : class, T;
    }

}
