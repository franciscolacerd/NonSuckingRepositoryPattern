using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NSRP.Application.Contracts.Persistence;
using NSRP.Application.Extensions;
using NSRP.Application.Models.Persistence;
using NSRP.Domain.Common;
using NSRP.Persistence.Repositories.Common;
using System.Linq.Expressions;

namespace NSRP.Persistence.Repositories
{
    public class GenericRepository<TEntity, TDto> : BaseRepository<TEntity, NsrpContext>, IGenericRepository<TEntity, TDto>
        where TEntity : BaseDomainEntity
        where TDto : class
    {
        private readonly NsrpContext _dbContext;
        protected IMapper _mapper { get; }

        public GenericRepository(NsrpContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<TDto?> AddAsync(TEntity entity)
        {
            var mappedEntity = _mapper.Map<TEntity>(entity);

            var result = await GetByIdAsync(entity.GetKey(_dbContext));

            if (result == null)
            {
                await _dbContext
                    .Set<TEntity>()
                    .AddAsync(mappedEntity);

                await _dbContext.SaveChangesAsync();
            }

            return await GetByIdAsync(mappedEntity.GetKey(_dbContext));
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entitytoDelete = await dbSet.FindAsync(new object[] { entity.Id });

            if (entitytoDelete == null)
                return;

            dbSet.Remove(entity);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<TDto>> GetAllAsync()
        {
            return await _dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<TDto?> GetByIdAsync(int id)
        {
            var key = _dbContext.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.Single().Name;

            if (key == null) { return null; }

            return await _dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .Where(x => id.Equals(EF.Property<long>(x, key)))
                .ProjectTo<TDto?>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TDto>> QueryAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await QueryAsync(predicate, null);
        }

        public async Task<IReadOnlyList<TDto>> QueryAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            var _query = SetFiltersToQuery(predicate, includes);

            return await _query
                 .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();
        }

        public async Task<PagedList<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await QueryAsync(page, pageSize, predicate, null);
        }

        public async Task<PagedList<TDto>> QueryAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            var _query = SetFiltersToQuery(predicate, includes);

            var count = await _query.ProjectTo<TDto>(_mapper.ConfigurationProvider).CountAsync();

            var result = await _query
                 .Skip(Skip(page, pageSize))
                 .Take(pageSize)
                 .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();

            return new PagedList<TDto>(count, page, pageSize, null!, null!, result);
        }

        public async Task<PagedList<TDto>> QueryAsync(
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection,
            Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await QueryAsync(page, pageSize, sortColumn, sortDirection, predicate, null!);
        }

        public async Task<PagedList<TDto>> QueryAsync(
            int page,
            int pageSize,
            string sortColumn,
            string sortDirection,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var _query = SetFiltersToQuery(predicate, includes);

            var count = await _query.ProjectTo<TDto>(_mapper.ConfigurationProvider).CountAsync();

            IReadOnlyList<TDto>? result = null;

            if (string.IsNullOrEmpty(sortColumn))
            {
                result = await _query
                 .OrderByDescending("CreatedDateUtc")
                 .Skip(Skip(page, pageSize))
                 .Take(pageSize)
                 .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();

                return new PagedList<TDto>(count, page, pageSize, sortColumn, sortDirection, result);
            }

            if (string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
            {
                result = await _query
                 .OrderByDescending(sortColumn.ToUpperCaseFirst())
                 .Skip(Skip(page, pageSize))
                 .Take(pageSize)
                 .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();

                return new PagedList<TDto>(count, page, pageSize, sortColumn, sortDirection, result);
            }
            else
            {
                result = await _query
                 .OrderBy(sortColumn.ToUpperCaseFirst())
                 .Skip(Skip(page, pageSize))
                 .Take(pageSize)
                 .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();

                return new PagedList<TDto>(count, page, pageSize, sortColumn, sortDirection, result);
            }
        }

        public async Task<TDto?> QueryFirstAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return await QueryFirstAsync(predicate, null);
        }

        public async Task<TDto?> QueryFirstAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[]? includes)
        {
            var _query = SetFiltersToQuery(predicate, includes);

            return await _query
                 .ProjectTo<TDto?>(_mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await _dbContext
                 .Set<TEntity>()
                 .FindAsync(new object[] { entity.Id });

            await _dbContext.SaveChangesAsync();
        }
    }
}
