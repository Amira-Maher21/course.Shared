using Microsoft.EntityFrameworkCore;
using  Shared.Application.RepositoryBase;
using  Shared.Application.DataQuery;
using  Shared.Application.Multitenant;
 using  Shared.Domain.Contracts;
using  Shared.Domain.Contracts.EntityCommonData;
using  Shared.Infrastructure.DataContext;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;

namespace  Shared.Infrastructure.RepositoryBase
{
    public abstract class QueryRepositoryBase<TEntity> : IQueryRepository<TEntity> where TEntity : class, IRootEntity
    {
        protected readonly DbContext _context;
        protected readonly ISqlQueryBuilder _queryBuilder;
        protected readonly DapperDbContext _dapper;
        protected readonly ITenantResolver _tenantResolver;
        protected readonly IExceptionManager _exceptionManager;
        protected readonly DbSet<TEntity> _dbSet;

        public QueryRepositoryBase(ISqlQueryBuilder queryBuilder, DapperDbContext dapper, DbContext context, ITenantResolver tenantResolver, IExceptionManager exceptionManager)
        {
            _context = context;
            _queryBuilder = queryBuilder;
            _dapper = dapper;
            _tenantResolver = tenantResolver;
            _exceptionManager = exceptionManager;
            _dbSet = _context.Set<TEntity>();
        }
        internal IQueryable<TEntity> GetAllQueryable()
        {
            var result = _dbSet.AsQueryable();
            if (typeof(TEntity) is IDeletable)
            {
                result = result.Where(t => ((IDeletable)t).Deleted == false);
            }
            return result;

        }

        internal IQueryable<TEntity> GetAllActiveQueryable(bool active = true)
        {
            var result = GetAllQueryable();
            if (typeof(TEntity) is IActivable)
            {
                result = result.Where(t => ((IActivable)t).Active == active);
            }
            return result;
        }

        public async Task<ReturnBase<IEnumerable<TEntity>>> GetAllAsync()
        {
            try
            {
                var result = await GetAllQueryable().ToListAsync();
                return ReturnBase<IEnumerable<TEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                return ReturnBase<IEnumerable<TEntity>>.Fail(ex, _exceptionManager);
            }
        }

        public async Task<ReturnBase<IEnumerable<TEntity>>> GetAllActiveAsync(bool active = true)
        {
            try
            {
                var result = await GetAllActiveQueryable(active).ToListAsync();
                return ReturnBase<IEnumerable<TEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                return ReturnBase<IEnumerable<TEntity>>.Fail(ex, _exceptionManager);
            }

        }

        protected async Task<ReturnBase<IEnumerable<TQueryResult>>> Query<TQueryResult>(IQueryObject<TQueryResult> queryObject, SqlQueryOptions queryOptions)
        {
            return await queryObject.Query(queryOptions);
        }

        protected async Task<ReturnBase<IEnumerable<TQueryResult>>> Query<TQueryResult>(IQueryObject<TQueryResult> queryObject, SqlQueryOptions queryOptions, object[] functionParameters)
        {
            return await queryObject.Query(queryOptions, functionParameters);
        }

        protected async Task<ReturnBase<TQueryResult>> QueryScalar<TQueryResult>(IScalarQueryObject<TQueryResult> queryObject, SqlQueryOptions queryOptions)
        {
            return await queryObject.Query(queryOptions);
        }
    }
}