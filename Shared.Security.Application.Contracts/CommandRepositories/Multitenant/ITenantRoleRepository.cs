using Shared.Kernel.BaseReturnTypes;

namespace Shared.Security.Application.Contracts.CommandRepositories.Multitenant
{
    public interface ITenantRoleRepository
    {
        Task<ReturnBase> CreateRoleAsync(string name, string tenantName);
    }
}
