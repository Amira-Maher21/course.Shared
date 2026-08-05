using Microsoft.EntityFrameworkCore;
 using  Shared.Application.RepositoryBase;
using  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP;
using   Shared.Application.SharedModels;
using  Shared.Domain.Contracts;
using  Shared.Domain.Contracts.EntityPEP;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;

namespace  Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityPEP
{
    internal class EntityPostingHelper<TEntity> : IEntityPostHelper where TEntity : class, IRootEntity, IPosting
    {
        private readonly ICommandRepository<TEntity> _repository;
        private readonly DbContext _context;
        private readonly IEntityPEPChecker<TEntity> _entityPEPChecker;
        private readonly IExceptionManager _exceptionManager;

        public EntityPostingHelper(ICommandRepository<TEntity> repository,
                                DbContext context,
                                IEntityPEPChecker<TEntity> entityPEPChecker,
                                IExceptionManager exceptionManager)
        {
            _repository = repository;
            _context = context;
            _entityPEPChecker = entityPEPChecker;
            _exceptionManager = exceptionManager;
        }
        public async Task<ReturnBase<bool>> Post(EntityKeyValueDictionary keys)
        {
            return await updateEntity(keys, true);

        }

        public async Task<ReturnBase<bool>> UnPost(EntityKeyValueDictionary keys)
        {
            return await updateEntity(keys, false);
        }

        private async Task<ReturnBase<bool>> updateEntity(EntityKeyValueDictionary keys, bool postState)
        {
            try
            {
                var getEntityResult = await _repository.GetEntityAsync(keys);
                if (getEntityResult.Succeeded)
                {
                    var entity = getEntityResult.Result;
                    var operationAllowed = postState ?
                                        _entityPEPChecker.CanPosting(entity!) :
                                        _entityPEPChecker.CanUnPosting(entity!);
                    if (operationAllowed)
                    {
                        entity!.POSTING = postState;
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
