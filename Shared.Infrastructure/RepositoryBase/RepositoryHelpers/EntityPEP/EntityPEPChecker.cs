using  Shared.Application.RepositoryBase.RepositoryHelpers.EntityPEP;
using  Shared.Domain.Contracts;
using  Shared.Domain.Contracts.EntityPEP;

namespace  Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityPEP
{
    internal class EntityPEPChecker<TEntity> : IEntityPEPChecker<TEntity> where TEntity : IRootEntity
    {
        public bool CanPricing(TEntity entity)
        {
            if (entity != null)
            {
                if (HasPricing(entity) && ((IPricing)entity).Pricing == false)
                {
                    return true;
                }
                return false;
            }
            throw new NullReferenceException(nameof(entity));
        }

        public bool CanUnPricing(TEntity entity)
        {
            if (entity != null)
            {
                if (CanUnEntry(entity)) return false;
                if (CanUnPosting(entity)) return false;

                if (HasPricing(entity) && ((IPricing)entity).Pricing == true)
                {
                    return true;
                }
                return false;
            }
            throw new NullReferenceException(nameof(entity));
        }

        public bool CanEntry(TEntity entity)
        {
            if (entity != null)
            {
                if (CanPricing(entity)) return false;
                if (HasEntry(entity) && ((IEntry)entity).Entry == false)
                {
                    return true;
                }
                return false;
            }
            throw new NullReferenceException(nameof(entity));
        }

        public bool CanUnEntry(TEntity entity)
        {
            if (entity != null)
            {
                if (CanUnPosting(entity)) return false;


                if (HasEntry(entity) && ((IEntry)entity).Entry == true)
                {
                    return true;
                }
                return false;
            }
            throw new NullReferenceException(nameof(entity));
        }


        public bool CanPosting(TEntity entity)
        {

            if (entity != null)
            {
                if (CanPricing(entity)) return false;
                if (CanEntry(entity)) return false;
                if (HasPosting(entity) && ((IPosting)entity).POSTING == false)
                {
                    return true;
                }
                return false;
            }
            throw new NullReferenceException(nameof(entity));
        }

        public bool CanUnPosting(TEntity entity)
        {
            if (entity != null)
            {
                if (HasPosting(entity) && ((IPosting)entity).POSTING == true)
                {
                    return true;
                }
                return false;
            }
            throw new NullReferenceException(nameof(entity));
        }

        public bool HasPricing(TEntity entity)
        {
            return entity is IPricing;
        }

        public bool HasEntry(TEntity entity)
        {
            return entity is IEntry;
        }

        public bool HasPosting(TEntity entity)
        {
            return entity is IPosting;
        }
    }
}
