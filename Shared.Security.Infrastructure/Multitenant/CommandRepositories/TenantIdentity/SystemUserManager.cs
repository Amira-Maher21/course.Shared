using Microsoft.AspNetCore.Identity;
using Shared.Security.Infrastructure.System.IdentityModels;
using Shared.Kernel.BaseReturnTypes;
using Shared.Kernel.Exceptions;
using Shared.Security.Application.Contracts.CommandRepositories.Multitenant;
using Shared.Security.Application.Contracts.DTO.System.Identity;
using Shared.Security.Domain.Models.Identity;
using Shared.Security.Infrastructure.Multitenant.DataContext;
using Shared.Security.Infrastructure.System.IdentityModels;
using Shared.Security.Infrastructure.System.IdentityModels;

namespace Shared.Security.Infrastructure.Multitenant.CommandRepositories.TenantIdentity
{
    public class SystemUserManager : ISystemUserManager
    {
        private readonly UserManager<SystemUser> _userManager;
        private readonly TenantDbContext _dbContext;
        private readonly IExceptionManager _exceptionManager;

        public SystemUserManager(UserManager<SystemUser> userManager, TenantDbContext dbContext, IExceptionManager exceptionManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            this._exceptionManager = exceptionManager;
        }

        public async Task<ReturnBase<List<string>>> GetRolesAsync(SystemUserDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null) return ReturnBase<List<string>>.Fail();
            var roles = await _userManager.GetRolesAsync(user);
            return ReturnBase<List<string>>.Success(roles.ToList());
        }

        public async Task<ReturnBase> RegisterAsync(RegisterSystemUserDto user)
        {
            SystemUser sysUser = new SystemUser
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(sysUser, user.Password);

            if (!result.Succeeded)
            {
                var errors = new List<ReturnBaseError>();
                foreach (var error in result.Errors)
                {
                    errors.Add(new ReturnBaseError { ErrorCode = error.Code, ErrorMessage = error.Description });
                }
                return ReturnBase.Fail(errors);
            }

            return ReturnBase.Success();
        }

        public async Task<ReturnBase> UpdateRefreshTokenAsync(string userId, string token, DateTime time)
        {
            try
            {
                var refreshTokenEntity = await _dbContext.RefreshTokens.FindAsync(userId);
                if (refreshTokenEntity != null)
                {
                    refreshTokenEntity.RefreshToken = token;
                    refreshTokenEntity.ExpiryTime = time;
                }
                else
                {
                    refreshTokenEntity = new UserRefreshToken
                    {
                        Id = userId,
                        RefreshToken = token,
                        ExpiryTime = time
                    };
                    await _dbContext.RefreshTokens.AddAsync(refreshTokenEntity);
                }
                await _dbContext.SaveChangesAsync();
                return ReturnBase.Success();

            }
            catch (Exception ex)
            {
                return ReturnBase.Fail(ex, this._exceptionManager);
            }
        }

        public async Task<ReturnBase<SystemUserDto>> ValidateUserAsync(SystemLoginDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            var result = (user != null && await _userManager.CheckPasswordAsync(user, login.Password));
            if (!result) return ReturnBase<SystemUserDto>.Fail();

            var systemUser = new SystemUserDto
            {
                UserId = user.Id,
                UserName = user.UserName
            };
            return ReturnBase<SystemUserDto>.Success(systemUser);
        }
    }
}