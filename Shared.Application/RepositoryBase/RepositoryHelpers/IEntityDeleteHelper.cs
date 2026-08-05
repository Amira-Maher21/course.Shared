using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.RepositoryBase.RepositoryHelpers
{
    public interface IEntityDeleteHelper<TEntity> where TEntity : class
    {
        ReturnBase<TEntity> DeleteEntity(TEntity entity, EntityStructure entityStructure);

    }
}
