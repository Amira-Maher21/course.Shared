using Microsoft.EntityFrameworkCore;
using  Shared.Application.API;
using  Shared.Application.Multitenant;
using  Shared.Application.RepositoryBase.RepositoryHelpers;
using  Shared.Domain.Contracts.EntityCommonData;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace  Shared.Infrastructure.RepositoryBase.RepositoryHelpers.EntityCommands
{
    internal class EntityUpdateHelper<TEntity> : IEntityUpdateHelper<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly IEntityKeyHelper _keyHelper;
        private readonly ITenantResolver _tenantResolver;
        private readonly IExceptionManager _exceptionManager;
        private readonly CommonUserData _userData;
        private readonly FiscalYearUpdateHelper _fiscalYearUpdateHelper;
        private readonly AuditableUpdateHelper _auditableUpdateHelper;

        public EntityUpdateHelper(DbContext context,
                                    IEntityKeyHelper keyHelper,
                                    ITenantResolver tenantResolver,
                                    IExceptionManager exceptionManager)
        {
            _context = context;
            _keyHelper = keyHelper;
            _tenantResolver = tenantResolver;
            _exceptionManager = exceptionManager;

            _fiscalYearUpdateHelper = new FiscalYearUpdateHelper();
            _auditableUpdateHelper = new AuditableUpdateHelper();

            _userData = _tenantResolver.GetCommonUserData();
        }

        public ReturnBase<TEntity> Update(TEntity updatedEntity, TEntity originalEntity, EntityStructure entityStructure)
        {
            try
            {
                _auditableUpdateHelper.ForUpdate(updatedEntity, originalEntity, _userData);

                // Detach original
                _context.Entry(originalEntity).State = EntityState.Detached;

                // Attach updated safely
                updatedEntity = AttachOrGetTracked((dynamic)updatedEntity);
                _context.Entry(updatedEntity).State = EntityState.Modified;

                if (entityStructure.RelatedEntities is not null && entityStructure.RelatedEntities.Any())
                {
                    updateRelatedEntities(updatedEntity, originalEntity, entityStructure.RelatedEntities);
                }


                return ReturnBase<TEntity>.Success(updatedEntity);

            }

            catch (Exception ex)
            {
                return ReturnBase<TEntity>.Fail(ex, _exceptionManager);
            }
        }

        private void updateRelatedEntities(object updatedEntity, object originalEntity, List<RelatedEntity> relatedEntities)
        {

            if (relatedEntities is not null && relatedEntities.Any())
            {
                foreach (var relatedEntity in relatedEntities)
                {
                    var navigationProperty = updatedEntity.GetType().GetProperty(relatedEntity.NavigationProperty!);
                    if (navigationProperty is null)
                    {
                        throw new Exception($"Property {relatedEntity.NavigationProperty!} doesn't exist in the current entity type [{updatedEntity.GetType().Name}].");

                    }
                    var updatedObject = navigationProperty.GetValue(updatedEntity);
                    var originalObject = navigationProperty.GetValue(originalEntity);

                    var updatedCollection = updatedObject as IEnumerable<object>;

                    var isCollection = typeof(IEnumerable).IsAssignableFrom(navigationProperty.PropertyType)
                                            && navigationProperty.PropertyType != typeof(string);

                    //if (updatedCollection is not null && updatedCollection.Any()) // if related entity is a collection
                    if (isCollection)
                    {
                        // Ensure we have a specific generic type
                        Type concreteType = typeof(EntityUpdateHelper<>).MakeGenericType(typeof(TEntity));

                        var allMethods = concreteType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                             .Select(m => m.Name)
                             .ToList();

                        // Find the method
                        MethodInfo? method = concreteType
                            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                            .FirstOrDefault(m => m.Name == "updateCollection" && m.IsGenericMethod);

                        if (method == null)
                        {
                            throw new InvalidOperationException("Generic method 'updateCollection' not found in EntityUpdateHelper<TEntity>.");
                        }


                        // Make the method generic
                        MethodInfo updateCollectionMethod = method.MakeGenericMethod(relatedEntity.EntityType!);

                        //MethodInfo updateCollectionMethod = typeof(EntityUpdateHelper<TEntity>).GetMethod("updateCollection", BindingFlags.NonPublic)!.MakeGenericMethod(relatedEntity.EntityType!);
                        updateCollectionMethod.Invoke(this, new object[] { updatedObject!, originalObject!, relatedEntity });
                    }
                    else
                    {
                        if (updatedObject is not null && originalEntity is not null) // updated
                        {
                            _auditableUpdateHelper.ForUpdate(updatedObject, originalEntity, _userData);
                            updatedObject = AttachOrGetTracked((dynamic)updatedObject);
                            _context.Entry(updatedObject).State = EntityState.Modified;

                            if (relatedEntity.RelatedEntities?.Any() == true)
                            {
                                updateRelatedEntities(updatedObject, originalObject, relatedEntity.RelatedEntities);
                            }


                        }
                        else if (updatedObject is not null && originalEntity is null) // inserted
                        {
                            _context.Add(updatedObject);
                        }
                        else if (updatedObject is null && originalEntity is not null) // deleted
                        {
                            _context.Remove(originalEntity);
                        }
                    }


                }
            }
        }

        private void updateCollection<TCollectionEntity>(ICollection<TCollectionEntity> updatedEntityCollection, ICollection<TCollectionEntity> originalEntityCollection, RelatedEntity relatedEntity)
            where TCollectionEntity : class
        {
            if (updatedEntityCollection is null) updatedEntityCollection = new List<TCollectionEntity>();
            if (originalEntityCollection is null) originalEntityCollection = new List<TCollectionEntity>();

            EntityEqualityComparer<TCollectionEntity> equalityComparer = new EntityEqualityComparer<TCollectionEntity>(_keyHelper);
            equalityComparer.Keys = relatedEntity.Keys;

            var matchedPairs = (
                from updated in updatedEntityCollection
                from original in originalEntityCollection
                where equalityComparer.Equals(updated, original)
                select (updated, original)
            ).ToList();

            // Now filter for real changes (not just key matches)
            var updatedPairs = matchedPairs
                .Where(pair => !AreEntitiesEqual(pair.updated, pair.original))
                .ToList();

            var updatedEntities = updatedPairs.Select(p => p.updated).ToList();
            var originalUpdatedEntities = updatedPairs.Select(p => p.original).ToList();

            // Existing keys — anything not matched by key is new (inserted)
            var matchedUpdatedEntities = matchedPairs.Select(p => p.updated).ToList();

            var insertedItems = updatedEntityCollection
                .Except(matchedUpdatedEntities, equalityComparer)
                .ToList();

            var updatedIds = updatedEntityCollection
                .Select(x => _keyHelper.GetEntityKeyAsString(x, relatedEntity.Keys.ToList()))
                .ToHashSet();

            var deletedItems = originalEntityCollection
                .Where(x => !updatedIds.Contains(_keyHelper.GetEntityKeyAsString(x, relatedEntity.Keys.ToList())))
                .ToList();

            var updatedItems = updatedPairs.Select(p => p.updated).ToList();
            var originalUpdatedItems = updatedPairs.Select(p => p.original).ToList();

            // Ensure we have a specific generic type
            Type concreteType = typeof(EntityUpdateHelper<>).MakeGenericType(typeof(TEntity));

            // Find the method
            MethodInfo? method = concreteType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "updateInsertedCollection" && m.IsGenericMethod);

            if (method == null)
            {
                throw new InvalidOperationException("Generic method 'updateInsertedCollection' not found in EntityUpdateHelper<TEntity>.");
            }

            MethodInfo updateInsertedMethod = method.MakeGenericMethod(relatedEntity.EntityType!);
            updateInsertedMethod.Invoke(this, new object[] { insertedItems });

            // Find the method
            method = concreteType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "updateUpdatedCollection" && m.IsGenericMethod);

            if (method == null)
            {
                throw new InvalidOperationException("Generic method 'updateUpdatedCollection' not found in EntityUpdateHelper<TEntity>.");
            }

            MethodInfo updateUpdatedMethod = method.MakeGenericMethod(relatedEntity.EntityType!);
            updateUpdatedMethod.Invoke(this, new object[] { updatedItems, originalUpdatedItems, relatedEntity });

            if (deletedItems.Any())
            {
                foreach (var entity in deletedItems)
                {
                    var trackedEntity = EnsureTrackedBeforeRemove(entity);
                    _context.Remove(trackedEntity); // Remove the tracked one
                }
            }

        }

        public bool CompareEntitiesByKeys<TEntity>(TEntity entity1, TEntity entity2, string[] keyProperties)
        where TEntity : class
        {
            var keys1 = _keyHelper.GetKeyPairs(entity1, keyProperties);
            var keys2 = _keyHelper.GetKeyPairs(entity2, keyProperties);
            return keys1.SequenceEqual(keys2);
        }


        private void updateInsertedCollection<TCollectionEntity>(ICollection<TCollectionEntity> updatedEntityCollection)
        {
            if (updatedEntityCollection is not null && updatedEntityCollection.Any())
            {
                foreach (var entity in updatedEntityCollection)
                {
                    if (entity is not null)
                    {
                        _fiscalYearUpdateHelper.UpdateFiscalYear(entity, _userData);
                        _auditableUpdateHelper.ForInsert(entity, _userData);

                        // Safely track entity, only if not already tracked
                        var trackedEntity = AttachOrGetTracked((dynamic)entity);

                        if (trackedEntity == null)
                        {
                            _context.Add(entity); // it's not tracked — safe to add
                        }

                        _context.Entry(trackedEntity).State = EntityState.Added;
                    }

                }
            }
        }

        private void updateUpdatedCollection<TCollectionEntity>(ICollection<TCollectionEntity> updatedEntityCollection, ICollection<TCollectionEntity> originalEntityCollection, RelatedEntity relatedEntity) where TCollectionEntity : class
        {
            if (updatedEntityCollection is not null && updatedEntityCollection.Any())
            {
                // Build a dictionary of original entities by their keys
                var originalEntitiesByKey = originalEntityCollection
                    .ToDictionary(x => _keyHelper.GetEntityKeyAsString(x, relatedEntity.Keys.ToList()));

                // Add or update entities in the context
                foreach (var entity in updatedEntityCollection)
                {
                    if (entity is not null)
                    {
                        var entityKey = _keyHelper.GetEntityKeyAsString(entity, relatedEntity.Keys.ToList());

                        if (originalEntitiesByKey.TryGetValue(entityKey, out var originalEntity))
                        {
                            // Set audit data
                            _auditableUpdateHelper.ForUpdate(entity, originalEntity, _userData);
                        }
                        else
                        {
                            // No matching original entity found
                            _auditableUpdateHelper.ForUpdate(entity, null, _userData);
                        }

                        // Attach or get the tracked entity
                        var tracked = AttachOrGetTracked((dynamic)entity);

                        // If the entity was not tracked, add it to the context
                        if (tracked == null)
                        {
                            _context.Add(entity); // Add new entities to the context
                        }
                        else
                        {
                            //If already tracked, apply any changes to it
                            _context.Entry(tracked).CurrentValues.SetValues(entity);

                            //Explicitly mark the entity as Modified to track the changes
                            _context.Entry(tracked).State = EntityState.Modified;
                        }
                    }
                }
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

        // Check for duplicate tracked entities.
        // If a duplicate is found, we use it instead of attaching new entity otherwise a new entity is attached safely.
        private TEntity AttachOrGetTracked<TEntity>(TEntity entity) where TEntity : class
        {
            var entityType = _context.Model.FindEntityType(typeof(TEntity));
            var keyProperties = entityType!.FindPrimaryKey()!.Properties;

            var existingEntry = _context.ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(e =>
                keyProperties.All(p =>
                {
                    var prop = typeof(TEntity).GetProperty(p.Name);
                    var value1 = prop?.GetValue(e.Entity);
                    var value2 = prop?.GetValue(entity);
                    return Equals(value1, value2);
                }));

            if (existingEntry != null)
            {
                return existingEntry.Entity;
            }

            _context.Attach(entity);
            return entity;
        }

        private T EnsureTrackedBeforeRemove<T>(T entity) where T : class
        {
            var entityType = _context.Model.FindEntityType(typeof(T))!;
            var key = entityType.FindPrimaryKey()!;

            var keyValues = key.Properties
                .Select(p => typeof(T).GetProperty(p.Name)?.GetValue(entity))
                .ToArray();

            // Try to find already tracked entity by key
            var tracked = _context.ChangeTracker
                .Entries<T>()
                .FirstOrDefault(e => key.Properties.All(p =>
                {
                    var prop = typeof(T).GetProperty(p.Name);
                    var originalVal = prop?.GetValue(e.Entity);
                    var incomingVal = prop?.GetValue(entity);
                    return Equals(originalVal, incomingVal);
                }))?.Entity;

            if (tracked != null)
            {
                return tracked;
            }

            // Otherwise try to get it from DbSet and attach
            var dbSet = _context.Set<T>();
            var existingEntity = dbSet.Find(keyValues);
            if (existingEntity != null)
            {
                _context.Attach(existingEntity);
                return existingEntity;
            }

            // If not found in DB, attach the given one
            _context.Attach(entity);
            return entity;
        }

        private bool AreEntitiesEqual<T>(T entity1, T entity2)
        {
            if (entity1 == null || entity2 == null) return false;

            var ignoredProperties = new HashSet<string> { "In_User", "In_Date", "Mod_User", "Mod_Date" };

            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Ignore keys or navigation properties or ignored properties
                if (!prop.CanRead || prop.GetMethod?.IsVirtual == true || ignoredProperties.Contains(prop.Name))
                    continue;

                var val1 = prop.GetValue(entity1);
                var val2 = prop.GetValue(entity2);

                if (val1 == null && val2 == null)
                    continue;

                if (val1 == null || val2 == null || !val1.Equals(val2))
                    return false;
            }

            return true;
        }

        private void PreserveCreationAuditFields<TCollectionEntity>(TCollectionEntity targetEntity, TCollectionEntity sourceEntity)
        {
            var entityType = typeof(TCollectionEntity);

            var creationProperties = new[] { "In_User", "In_Date" };

            foreach (var propName in creationProperties)
            {
                var prop = entityType.GetProperty(propName);

                if (prop is not null && prop.CanRead && prop.CanWrite)
                {
                    var originalValue = prop.GetValue(sourceEntity);
                    prop.SetValue(targetEntity, originalValue);
                }
            }
        }
    }

    public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : class
    {
        private readonly IEntityKeyHelper _keyHelper;

        public EntityEqualityComparer(IEntityKeyHelper keyHelper)
        {
            _keyHelper = keyHelper;
        }
        public string[]? Keys { get; set; }
        public bool Equals(TEntity? x, TEntity? y)
        {
            if (x != null && y != null)
            {


                var xKeys = _keyHelper.GetKeyPairs(x, Keys!);
                var yKeys = _keyHelper.GetKeyPairs(y, Keys!);
                return xKeys.SequenceEqual(yKeys);
            }
            return false;
        }

        public int GetHashCode([DisallowNull] TEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}
