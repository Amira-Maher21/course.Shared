using  Shared.Application.SharedModels;
using  Shared.Domain.Contracts;
using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.RepositoryBase
{
    public interface ICommandRepository<TEntity> where TEntity : IRootEntity
    {
        Task<ReturnBase<TEntity>> GetEntityAsync(EntityKeyValueDictionary keys);
        Task<ReturnBase<TEntity>> InsertAsync(TEntity entity);

        Task<ReturnBase<TEntity>> UpdateAsync(TEntity updatedEntity);
        Task<ReturnBase<TEntity>> DeleteAsync(EntityKeyValueDictionary keys);


        Task<ReturnBase<TEntity>> DeleteAsync<TKey>(TKey keys);
        Task<ReturnBase<IEnumerable<ReturnBase<TEntity>>>> BulkDeleteAsync(IEnumerable<EntityKeyValueDictionary> keysList, Func<Task<ReturnBase<int>>> saveFunc);
        //Task<ReturnBase<string>> AutoIncrementAsync(string tableName, string code, long? groupValue = null, string group = "");
    }
}
