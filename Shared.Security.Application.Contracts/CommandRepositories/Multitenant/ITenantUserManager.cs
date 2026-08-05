using  Shared.Security.Application.Contracts.DTO.Multitenant.Identity;
using Shared.Kernel.BaseReturnTypes;
using  Shared.Security.Application.Contracts.DTO.Multitenant.Identity;

namespace  Shared.Security.Application.Contracts.CommandRepositories.Multitenant
{
    public interface ITenantUserManager
    {
        Task<ReturnBase> RegisterAsync(RegisterTenantUserDto user, string tenantName);
        Task<ReturnBase<TenantUserDto>> ValidateUserAsync(TenantLoginDto login);

        Task<ReturnBase<List<string>>> GetRolesAsync(TenantUserDto login);

        Task<ReturnBase> UpdateRefreshTokenAsync(string userId, string token, DateTime time);

    }
}
