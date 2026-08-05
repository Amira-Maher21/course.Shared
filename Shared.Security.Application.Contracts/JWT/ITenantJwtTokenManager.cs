
using Shared.Kernel.BaseReturnTypes;
using Shared.Security.Application.Contracts.DTO.JWT;
using Shared.Security.Application.Contracts.DTO.Multitenant.Identity;

namespace NDS.Shared.Security.Application.Contracts.JWT
{
    public interface ITenantJwtTokenManager
    {
        Task<ReturnBase<CreateTokenResultDto>> CreateTokenAsync(TenantLoginDto login);
    }
}
