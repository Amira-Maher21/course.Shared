using Shared.Security.Application.Contracts.RepositoryContracts;
using  Shared.Domain.Models.Multitenant;
using  Shared.Security.Application.Contracts.RepositoryContracts;

namespace Shared.Security.Application.Contracts.CommandRepositories.Multitenant
{
    public interface ITenantRepository : ISecurityCommandRepository<Tenant>
    {
    }
}
