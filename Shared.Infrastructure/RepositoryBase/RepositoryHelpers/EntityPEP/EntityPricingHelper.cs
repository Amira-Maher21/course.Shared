using Microsoft.EntityFrameworkCore;
using  Shared.Application.RepositoryBase;
using  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP;
using  Shared.Application.SharedModels;
using  Shared.Domain.Contracts;
using  Shared.Domain.Contracts.EntityPEP;
using  Shared.Kernel.BaseReturnTypes;
using   Shared.Kernel.Exceptions;
using Shared.Application.RepositoryBase;
using  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP;

namespace  Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityPEP
{
    internal class EntityPricingHelper<TEntity> : IEntityPricingHelper where TEntity : class, IRootEntity, IPricing
    {
        private readonly ICommandRepository<TEntity> _repository;
        private readonly DbContext _context;
        private readonly IEntityPEPChecker<TEntity> _entityPEPChecker;
        private readonly IExceptionManager _exceptionManager;

        public EntityPricingHelper(ICommandRepository<TEntity> repository,
                                DbContext context,
                                IEntityPEPChecker<TEntity> entityPEPChecker,
                                IExceptionManager exceptionManager)
        {
            _repository = repository;
            _context = context;
            _entityPEPChecker = entityPEPChecker;
            _exceptionManager = exceptionManager;
        }
        public Task<ReturnBase<bool>> Price(EntityKeyValueDictionary keys)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnBase<bool>> UnPrice(EntityKeyValueDictionary keys)
        {
            throw new NotImplementedException();
        }

        private async Task<ReturnBase<bool>> updateEntity(EntityKeyValueDictionary keys, bool pricingState)
        {
            try
            {
                var getEntityResult = await _repository.GetEntityAsync(keys);
                if (getEntityResult.Succeeded)
                {
                    var entity = getEntityResult.Result;
                    var operationAllowed = pricingState ?
                                        _entityPEPChecker.CanPricing(entity!) :
                                        _entityPEPChecker.CanUnPricing(entity!);
                    if (operationAllowed)
                    {
                        entity!.Pricing = pricingState;
                        await _context.SaveChangesAsync();
                        return ReturnBase<bool>.Success(true);

                    }
                    else
                    {

                        return ReturnBase<bool>.Fail(new Exception("Operation not allowed for this entity"), _exceptionManager);
                    }
                }
                else
                {
                    return ReturnBase<bool>.Fail(getEntityResult.Errors);
                }
            }
            catch (Exception ex)
            {
                return ReturnBase<bool>.Fail(ex, _exceptionManager);
            }
        }

    }
}
