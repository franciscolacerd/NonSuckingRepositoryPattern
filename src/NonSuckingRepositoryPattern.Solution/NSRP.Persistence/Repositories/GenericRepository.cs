using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NSRP.Application.Contracts.Persistence;
using NSRP.Application.DTOs.Common;
using NSRP.Application.Extensions;
using NSRP.Application.Models.Pagination;
using NSRP.Domain.Common;
using NSRP.Persistence.Repositories.Common;
using System.Linq.Expressions;

namespace NSRP.Persistence.Repositories
{
    public class GenericRepository<TEntity, TDto, TContext> : BaseRepository<TEntity, TContext>, IGenericRepository<TEntity, TDto>
        where TEntity : BaseDomainEntity
        where TDto : BaseDto
        where TContext : BaseDbContext
    {
        private readonly TContext _dbContext;
        protected IMapper _mapper { get; }

        public GenericRepository(TContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var entityEntry = await _dbContext
                .Set<TEntity>()
                .AddAsync(entity);

            return entityEntry.Entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entitytoDelete = await dbSet.FindAsync(new object[] { entity.Id });

            if (entitytoDelete == null)
                return;

            dbSet.Remove(entity);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            return await _dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            var key = _dbContext.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.Single().Name;

            if (key == null) { return null; }

            return await _dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .Where(x => id.Equals(EF.Property<long>(x, key)))
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await QueryAsync(predicate, null);
        }

        public async Task<IReadOnlyList<TEntity>> QueryAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            var _query = SetFiltersToQuery(predicate, includes);

            return await _query.ToListAsync();
        }

        public async Task<PagedResult<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null)
        {
            var _query = SetFiltersToQuery(predicate);

            return await _query.GetPagedAsync<TEntity, TDto>(page, pageSize, _mapper);
        }

        public async Task<PagedResult<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            var _query = SetFiltersToQuery(predicate, includes);

            return await _query.GetPagedAsync<TEntity, TDto>(page, pageSize, _mapper);
        }

        public async Task<PagedResult<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var _query = SetFiltersToQuery(predicate, includes, orderBy);

            return await _query.GetPagedAsync<TEntity, TDto>(page, pageSize, _mapper);
        }

        public async Task<TEntity?> QueryFirstAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await QueryFirstAsync(predicate, null);
        }

        public async Task<TEntity?> QueryFirstAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            var _query = SetFiltersToQuery(predicate, includes);

            return await _query.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await _dbContext
                 .Set<TEntity>()
                 .FindAsync(new object[] { entity.Id });
        }
    }
}
