using Shared.Security.Application.Contracts.DTO.Multitenant.Identity;

namespace Shared.Security.Application.Contracts.Services.Clients.UserCodeClient
{
    public interface IERPUserCodeService
    {
        Task<long?> CreateERPUserAsync(RegisterTenantUserDto user);
    }
}