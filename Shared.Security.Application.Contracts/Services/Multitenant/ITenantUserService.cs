using  Shared.Kernel.BaseReturnTypes;
using  Shared.Security.Application.Contracts.DTO.JWT;
using  Shared.Security.Application.Contracts.DTO.Multitenant.Identity;

namespace Shared.Security.Application.Contracts.Services.Multitenant
{
    public interface ITenantUserService
    {
        Task<ReturnBase> RegisterAsync(RegisterTenantUserDto user/*, string tenantName*/);
        Task<ReturnBase<LoginTokenResultDto>> LoginAsync(TenantLoginDto login);


    }
}
