using Shared.Kernel.BaseReturnTypes;
using Shared.Security.Application.Contracts.DTO.Multitenant.Tenants;

namespace Shared.Security.Application.Contracts.Services.Multitenant
{
    public interface ITenantService
    {
        Task<ReturnBase> RegisterTenant(RegisterTenantDto tenant);
    }
}
