using Shared.Domain.Contracts;
using Shared.Kernel.BaseReturnTypes;

namespace Shared.Security.Application.Contracts.RepositoryContracts
{
    public interface ISecurityQueryRepository<TEntity> where TEntity : IRootEntity
    {

        Task<ReturnBase<IEnumerable<TEntity>>> GetAllAsync();

        Task<ReturnBase<IEnumerable<TEntity>>> GetAllActiveAsync(bool active);
    }
}
