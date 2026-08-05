using Shared.Security.Application.Contracts.DTO.System.Identity;
using Shared.Kernel.BaseReturnTypes;
using Shared.Security.Application.Contracts.DTO.System.Identity;

namespace Shared.Security.Application.Contracts.CommandRepositories.Multitenant
{
    public interface ISystemUserManager
    {
        Task<ReturnBase> UpdateRefreshTokenAsync(string userId, string token, DateTime time);
        Task<ReturnBase<SystemUserDto>> ValidateUserAsync(SystemLoginDto login);
        Task<ReturnBase<List<string>>> GetRolesAsync(SystemUserDto login);
        Task<ReturnBase> RegisterAsync(RegisterSystemUserDto user);
    }
}
