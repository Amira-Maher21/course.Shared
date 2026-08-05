 using Shared.Security.Application.Contracts.RepositoryContracts;
using Shared.Domain.Models.Multitenant;

namespace  Shared.Security.Application.Contracts.QueryRepositories
{
    public interface ITenantQueryRepository : ISecurityQueryRepository<Tenant>
    {

    }
}
