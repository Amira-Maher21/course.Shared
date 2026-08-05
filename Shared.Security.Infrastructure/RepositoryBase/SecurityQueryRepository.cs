using  Shared.Domain.Contracts;
using  Shared.Kernel.BaseReturnTypes;
using  Shared.Security.Application.Contracts.RepositoryContracts;

namespace  Shared.Security.Infrastructure.RepositoryBase
{
    internal class SecurityQueryRepository<TEntity> : ISecurityQueryRepository<TEntity> where TEntity : IRootEntity
    {
        public Task<ReturnBase<IEnumerable<TEntity>>> GetAllActiveAsync(bool active)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnBase<IEnumerable<TEntity>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
