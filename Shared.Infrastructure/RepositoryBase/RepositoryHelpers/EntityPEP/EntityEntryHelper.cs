using Microsoft.EntityFrameworkCore;
using NDS.Shared.Application.RepositoryBase;
using NDS.Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP;
using  Shared.Application.RepositoryBase;
using  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP;
using  Shared.Application.SharedModels;
using  Shared.Domain.Contracts;
using  Shared.Domain.Contracts.EntityPEP;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;

namespace  Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityPEP
{
    internal class EntityEntryHelper<TEntity> : IEntityEntryHelper where TEntity : class, IRootEntity, IEntry
    {
        private readonly ICommandRepository<TEntity> _repository;
        private readonly DbContext _context;
        private readonly IEntityPEPChecker<TEntity> _entityPEPChecker;
        private readonly IExceptionManager _exceptionManager;

        public EntityEntryHelper(ICommandRepository<TEntity> repository,
                                DbContext context,
                                IEntityPEPChecker<TEntity> entityPEPChecker,
                                IExceptionManager exceptionManager)
        {
            _repository = repository;
            _context = context;
            _entityPEPChecker = entityPEPChecker;
            _exceptionManager = exceptionManager;
        }
        public async Task<ReturnBase<bool>> Entry(EntityKeyValueDictionary keys)
        {
            return await updateEntity(keys, true);
        }

        public async Task<ReturnBase<bool>> UnEntry(EntityKeyValueDictionary keys)
        {
            return await updateEntity(keys, false);
        }

        private async Task<ReturnBase<bool>> updateEntity(EntityKeyValueDictionary keys, bool entryState)
        {
            try
            {
                var getEntityResult = await _repository.GetEntityAsync(keys);
                if (getEntityResult.Succeeded)
                {
                    var entity = getEntityResult.Result;
                    var operationAllowed = entryState ?
                                        _entityPEPChecker.CanEntry(entity!) :
                                        _entityPEPChecker.CanUnEntry(entity!);
                    if (operationAllowed)
                    {
                        entity!.Entry = entryState;
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
