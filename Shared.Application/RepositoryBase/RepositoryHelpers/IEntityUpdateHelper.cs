 using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.RepositoryBase.RepositoryHelpers
{
    public interface IEntityUpdateHelper<TEntity> where TEntity : class
    {
        ReturnBase<TEntity> Update(TEntity updatedEntity, TEntity originalEntity, EntityStructure entityStructure);
    }
}
