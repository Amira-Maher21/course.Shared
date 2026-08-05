using  Shared.Domain.Contracts;

namespace  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP
{
    public interface IEntityPEPChecker<TEntity> where TEntity : IRootEntity
    {
        bool CanPricing(TEntity entity);
        bool CanUnPricing(TEntity entity);
        bool CanEntry(TEntity entity);
        bool CanUnEntry(TEntity entity);
        bool CanPosting(TEntity entity);
        bool CanUnPosting(TEntity entity);

        bool HasPricing(TEntity entity);
        bool HasEntry(TEntity entity);
        bool HasPosting(TEntity entity);


    }
}
