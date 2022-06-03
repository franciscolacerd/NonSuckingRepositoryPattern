using NSRP.Application.Models.Persistence;
using System.Linq.Expressions;

namespace NSRP.Application.Contracts.Persistence
{
    public interface IGenericRepository<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {

        Task<TDto?> AddAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
        Task<IReadOnlyList<TDto>> GetAllAsync();

        Task<TDto?> GetByIdAsync(int id);

        Task<IReadOnlyList<TDto>> QueryAsync(Expression<Func<TEntity, bool>>? predicate = null);

        Task<IReadOnlyList<TDto>> QueryAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes);

        Task<PagedList<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null);

        Task<PagedList<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes);

        Task<PagedList<TDto>> QueryAsync(
           int page,
           int pageSize,
           string sortColumn,
           string sortDirection,
           Expression<Func<TEntity, bool>>? predicate = null);

        Task<PagedList<TDto>> QueryAsync(
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TDto?> QueryFirstAsync(Expression<Func<TEntity, bool>>? predicate = null);

        Task<TDto?> QueryFirstAsync(Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes);

        Task UpdateAsync(TEntity entity);
    }
}
