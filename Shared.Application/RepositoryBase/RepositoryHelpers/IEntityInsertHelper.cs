using  Shared.Kernel.BaseReturnTypes;

namespace Shared.Application.RepositoryBase.RepositoryHelpers
{
    public interface IEntityInsertHelper<TEntity> where TEntity : class
    {
        ReturnBase<TEntity> InsertEntityAsync(TEntity entity, EntityStructure entityStructure);
    }
}
