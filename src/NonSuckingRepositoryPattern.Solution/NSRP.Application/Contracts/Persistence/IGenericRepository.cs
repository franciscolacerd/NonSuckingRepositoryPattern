using NSRP.Application.Models.Pagination;
using System.Linq.Expressions;

namespace NSRP.Application.Contracts.Persistence
{
    public interface IGenericRepository<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {

        Task<TEntity> AddAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        Task<TEntity?> GetByIdAsync(int id);

        Task UpdateAsync(TEntity entity);

        Task<IReadOnlyList<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? predicate = null);

        Task<IReadOnlyList<TEntity>> QueryAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes);

        Task<PagedResult<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null);

        Task<PagedResult<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes);

        Task<PagedResult<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> QueryFirstAsync(Expression<Func<TEntity, bool>>? predicate = null);

        Task<TEntity?> QueryFirstAsync(Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes);
    }
}
