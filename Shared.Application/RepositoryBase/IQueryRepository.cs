using  Shared.Domain.Contracts;
using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.RepositoryBase
{
    public interface IQueryRepository<TEntity> where TEntity : IRootEntity
    {
        Task<ReturnBase<IEnumerable<TEntity>>> GetAllAsync();
        Task<ReturnBase<IEnumerable<TEntity>>> GetAllActiveAsync(bool active);
    }
}
