using  Shared.Application.SharedModels;

namespace  Shared.Application.RepositoryBase.RepositoryHelpers
{
    public interface IEntityKeyHelper
    {
        EntityKeyValueDictionary GetKeyPairs(object entity, string[] keyNames);
        string GetEntityKeyAsString<T>(T entity, List<string> keys) where T : class;

    }
}
