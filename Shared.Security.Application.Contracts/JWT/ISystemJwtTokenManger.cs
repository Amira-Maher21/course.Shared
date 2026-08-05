 
using Shared.Kernel.BaseReturnTypes;
using Shared.Security.Application.Contracts.DTO.JWT;
using Shared.Security.Application.Contracts.DTO.System.Identity;

namespace NDS.Shared.Security.Application.Contracts.JWT
{
    public interface ISystemJwtTokenManger
    {
        Task<ReturnBase<CreateTokenResultDto>> CreateTokenAsync(SystemLoginDto login);

    }
}
