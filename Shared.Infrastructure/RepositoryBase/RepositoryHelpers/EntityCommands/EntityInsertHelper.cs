 using  Shared.Application.API;
using  Shared.Application.Multitenant;
using  Shared.Application.RepositoryBase.RepositoryHelpers;
using  Shared.Domain.Contracts.EntityCommonData;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;

namespace  Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityCommands
{
    internal class EntityInsertHelper<TEntity> : IEntityInsertHelper<TEntity> where TEntity : class
    {
        private readonly ITenantResolver _tenantResolver;
        private readonly IExceptionManager _exceptionManager;
        private readonly CommonUserData _userData;
        private readonly FiscalYearUpdateHelper _fiscalYearUpdateHelper;
        private readonly AuditableUpdateHelper _auditableUpdateHelper;

        public EntityInsertHelper(ITenantResolver tenantResolver, IExceptionManager exceptionManager)
        {

            _tenantResolver = tenantResolver;
            _exceptionManager = exceptionManager;
            _fiscalYearUpdateHelper = new FiscalYearUpdateHelper();
            _auditableUpdateHelper = new AuditableUpdateHelper();

            _userData = _tenantResolver.GetCommonUserData();
        }
        public ReturnBase<TEntity> InsertEntityAsync(TEntity entity, EntityStructure entityStructure)
        {
            try
            {
                if (entityStructure is null) throw new Exception("Entity Structure must be defined!!.");

                _fiscalYearUpdateHelper.UpdateFiscalYear(entity, _userData);

                _auditableUpdateHelper.ForInsert(entity, _userData);

                if (entityStructure.RelatedEntities.Any())
                {
                    foreach (var relatedEntity in entityStructure.RelatedEntities)
                    {
                        UpdateCommonFieldsForInsert(entity, relatedEntity);
                    }
                }
                return ReturnBase<TEntity>.Success(entity);
            }
            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex, _exceptionManager);

            }

        }
        protected void UpdateCommonFieldsForInsert(object entity, RelatedEntity relatedEntity)
        {

            var navigationProperty = entity.GetType().GetProperty(relatedEntity.NavigationProperty!);
            if (navigationProperty is null)
            {
                throw new Exception($"Property {relatedEntity.NavigationProperty!} doesn't exist in the current entity type [{entity.GetType().Name}].");

            }
            var propertyObject = navigationProperty.GetValue(entity);
            if (propertyObject is not null)
            {
                var collection = propertyObject as IEnumerable<object>;
                if (collection is not null && collection.Any())
                {
                    updateCollectionCommonFieldsForInsert(collection, relatedEntity.RelatedEntities);
                }
                else
                {
                    _fiscalYearUpdateHelper.UpdateFiscalYear(propertyObject, _userData);
                    _auditableUpdateHelper.ForInsert(propertyObject, _userData);

                    if (relatedEntity.RelatedEntities is not null && relatedEntity.RelatedEntities.Any())
                    {
                        foreach (var subRelatedEntity in relatedEntity.RelatedEntities)
                        {
                            UpdateCommonFieldsForInsert(propertyObject, subRelatedEntity);
                        }

                    }
                }
            }
        }

        private void updateCollectionCommonFieldsForInsert(IEnumerable<object> collection, List<RelatedEntity>? relatedEntities)
        {
            if (collection is not null && collection.Any())
            {
                bool updateFiscalYear = false;
                bool updateAuditable = false;
                if (collection.First() is IFiscalYear)
                {
                    updateFiscalYear = true;
                }

                if (collection.First() is IAuditable)
                {
                    updateAuditable = true;
                }

                foreach (var item in collection)
                {
                    if (item is not null)
                    {
                        if (relatedEntities is not null && relatedEntities.Any())
                        {
                            foreach (var subRelatedEntity in relatedEntities)
                            {
                                UpdateCommonFieldsForInsert(item, subRelatedEntity);
                            }

                        }
                        if (updateFiscalYear) _fiscalYearUpdateHelper.UpdateFiscalYear(item, _userData);
                        if (updateAuditable) _auditableUpdateHelper.ForInsert(item, _userData);
                    }
                }

            }

        }
    }
}
