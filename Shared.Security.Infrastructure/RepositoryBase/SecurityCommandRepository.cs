using Microsoft.EntityFrameworkCore;
using Shared.Application.Multitenant;
using  Shared.Application.SharedModels;
using  Shared.Domain.Contracts;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;
using  Shared.Kernel.Extensions;
using  Shared.Security.Application.Contracts.RepositoryContracts;
using Shared.Application.Multitenant;
using Shared.Application.SharedModels;
using Shared.Kernel.BaseReturnTypes;
using Shared.Kernel.Exceptions;
using Shared.Kernel.Extensions;

namespace Shared.Security.Infrastructure.RepositoryBase
{
    internal class SecurityCommandRepository<TEntity> : ISecurityCommandRepository<TEntity> where TEntity : class, IRootEntity
    {
        protected readonly DbContext _context;
        protected readonly ITenantResolver _tenantResolver;
        protected readonly IExceptionManager _exceptionManager;

        protected readonly DbSet<TEntity> _dbSet;

        public SecurityCommandRepository(DbContext context, ITenantResolver tenantResolver, IExceptionManager exceptionManager)
        {
            _context = context;
            _tenantResolver = tenantResolver;
            _exceptionManager = exceptionManager;

            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<ReturnBase<TEntity>> GetEntityAsync(EntityKeyValueDictionary keys)
        {
            try
            {
                var query = _dbSet.AsQueryable();
                query = query.GetByKeys(keys);
                var findResult = await query.ToListAsync();
                if (findResult is not null && findResult.Any())
                    return ReturnBase<TEntity>.Success(findResult.First());
                return ReturnBase<TEntity>.Success(null);
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex, _exceptionManager);
            }

        }

        public async Task<ReturnBase<TEntity>> InsertAsync(TEntity entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return ReturnBase<TEntity>.Success(entity);
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex, _exceptionManager);

            }
        }

        public async Task<ReturnBase<TEntity>> UpdateAsync(TEntity updatedEntity, EntityKeyValueDictionary keys)
        {
            try
            {
                var getEntityResult = await GetEntityAsync(keys);
                _dbSet.Update(updatedEntity);
                return ReturnBase<TEntity>.Success(updatedEntity);
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex, _exceptionManager);

            }
        }

        public virtual async Task<ReturnBase<TEntity>> DeleteAsync(EntityKeyValueDictionary keys)
        {
            try
            {

                var getEntityResult = await GetEntityAsync(keys);
                if (getEntityResult.Succeeded && getEntityResult.Result is not null)
                {
                    TEntity entity = getEntityResult.Result;

                    _dbSet.Remove(entity);
                    return ReturnBase<TEntity>.Success(entity);
                }

                return getEntityResult;
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex, _exceptionManager);

            }
        }


    }
}
