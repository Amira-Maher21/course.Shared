using Shared.Application.RepositoryBase.RepositoryHelpers;
using Shared.Application.SharedModels;

namespace  Shared.Infrastructure.RepositoryBase.RepositoryHelpers
{
    internal class EntityKeyHelper : IEntityKeyHelper
    {
        public EntityKeyValueDictionary GetKeyPairs(object entity, string[] keyNames)
        {
            var keys = new EntityKeyValueDictionary();
            if (keyNames is not null && keyNames.Any())
            {
                foreach (var key in keyNames)
                {
                    var property = entity.GetType().GetProperty(key);

                    if (property is null)
                    {
                        throw new Exception($"Entity {entity.GetType().Name} doesn't have a key property '{key}'!!.");
                    }

                    keys.Add(new KeyValuePair<string, object>(key, property.GetValue(entity)!));
                }
            }
            else
            {
                throw new Exception("Entity keys property must be defined!!. ");
            }
            return keys;
        }

        public string GetEntityKeyAsString<T>(T entity, List<string> keys) where T : class
        {
            // Get the type of the entity
            var entityType = typeof(T);

            // Create a list to hold the key values
            var keyValues = new List<string>();

            // Iterate over the keys and get the values of the corresponding properties in the entity
            foreach (var key in keys)
            {
                var property = entityType.GetProperty(key);
                if (property != null)
                {
                    var value = property.GetValue(entity, null);
                    keyValues.Add(value?.ToString() ?? string.Empty); // Handle null values gracefully
                }
            }

            // Combine the key values into a single string
            return string.Join(",", keyValues); // You can change the separator as needed
        }
    }
}
