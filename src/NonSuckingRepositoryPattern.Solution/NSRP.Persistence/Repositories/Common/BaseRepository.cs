using Microsoft.EntityFrameworkCore;
using NSRP.Domain.Common;
using System.Linq.Expressions;

namespace NSRP.Persistence.Repositories.Common
{
    public class BaseRepository<TEntity, TContext>
           where TEntity : BaseDomainEntity
           where TContext : DbContext
    {
        private readonly TContext _dbContext;

        public BaseRepository(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> SetFiltersToQuery(Expression<Func<TEntity, bool>>? predicate)
        {
            return SetFiltersToQuery(predicate, null);
        }

        public IQueryable<TEntity> SetFiltersToQuery(
             Expression<Func<TEntity, bool>>? predicate,
             Expression<Func<TEntity, object>>[]? includes)
        {
            return SetFiltersToQuery(predicate, includes, null);
        }

        public IQueryable<TEntity> SetFiltersToQuery(
             Expression<Func<TEntity, bool>>? predicate,
             Expression<Func<TEntity, object>>[]? includes,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            var dbSet = _dbContext.Set<TEntity>();

            var _query = dbSet.AsNoTracking();

            if (predicate != null)
                _query = _query.Where(predicate);

            if (includes != null)
                _query = includes.Aggregate(_query, (current, include) => current.Include(include));

            if (orderBy != null)
                return orderBy(_query);

            return _query;
        }
    }
}
