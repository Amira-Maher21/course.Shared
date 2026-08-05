using  Shared.Application.SharedModels;
using  Shared.Domain.Contracts;
using  Shared.Kernel.BaseReturnTypes;

namespace Shared.Security.Application.Contracts.RepositoryContracts
{
    public interface ISecurityCommandRepository<TEntity> where TEntity : IRootEntity
    {
        Task<ReturnBase<TEntity>> GetEntityAsync(EntityKeyValueDictionary keys);
        Task<ReturnBase<TEntity>> InsertAsync(TEntity entity);

        Task<ReturnBase<TEntity>> UpdateAsync(TEntity updatedEntity, EntityKeyValueDictionary keys);
        Task<ReturnBase<TEntity>> DeleteAsync(EntityKeyValueDictionary keys);

    }
}
