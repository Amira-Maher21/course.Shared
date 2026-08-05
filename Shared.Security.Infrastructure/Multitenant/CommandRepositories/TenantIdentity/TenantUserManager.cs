using Microsoft.AspNetCore.Identity;
using Shared.Security.Infrastructure.Multitenant.IdentityModels;
using Shared.Domain.SystemConstants;
using Shared.Kernel.BaseReturnTypes;
using Shared.Kernel.Exceptions;
using Shared.Security.Application.Contracts.CommandRepositories.Multitenant;
using Shared.Security.Application.Contracts.DTO.Multitenant.Identity;
using Shared.Security.Application.Contracts.Services.Clients.UserCodeClient;
using Shared.Security.Domain.Models.Identity;
using Shared.Security.Infrastructure.Multitenant.DataContext;
using Shared.Security.Infrastructure.Multitenant.IdentityModels;
using System.Data;
using Shared.Domain.SystemConstants;

namespace Shared.Security.Infrastructure.Multitenant.CommandRepositories.TenantIdentity
{
    public class TenantUserManager : ITenantUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TenantDbContext _dbContext;
        private readonly IExceptionManager _exceptionManager;
        private readonly IERPUserCodeService _erpService;
        public TenantUserManager(UserManager<ApplicationUser> userManager, TenantDbContext dbContext, IExceptionManager exceptionManager, IERPUserCodeService erpService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _exceptionManager = exceptionManager;
            _erpService = erpService;
        }

        public async Task<ReturnBase<List<string>>> GetRolesAsync(TenantUserDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null) return ReturnBase<List<string>>.Fail();
            var roles = await _userManager.GetRolesAsync(user);
            return ReturnBase<List<string>>.Success(roles.ToList());
        }
        public async Task<ReturnBase> RegisterAsync(RegisterTenantUserDto user, string tenantName)
        {
            try
            {
                var erpUserId = await _erpService.CreateERPUserAsync(user);

                if (erpUserId == null)
                {
                    return ReturnBase.Fail(new List<ReturnBaseError>
            {
                new ReturnBaseError
                {
                    ErrorCode = "ERP_ERROR",
                    ErrorMessage = "Failed to create ERP User"
                }
            });
                }

                ApplicationUser appUser = new ApplicationUser
                {
                    ERPUserCodeId = erpUserId,
                    TenantUserName = user.UserName,
                    UserName = TenantHelper.GetNameWithTenant(tenantName, user.UserName),
                    TenantName = tenantName,
                    Active = user.Active,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Tenant_ID = user.TenantId
                };

                var result = await _userManager.CreateAsync(appUser, user.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => new ReturnBaseError
                    {
                        ErrorCode = e.Code,
                        ErrorMessage = e.Description
                    }).ToList();

                    return ReturnBase.Fail(errors);
                }

                var roles = user.Roles?
                    .Select(r => TenantHelper.GetNameWithTenant(tenantName, r))
                    .ToList();

                if (roles != null && roles.Any())
                {
                    await _userManager.AddToRolesAsync(appUser, roles);
                }

                return ReturnBase.Success();
            }
            catch (Exception ex)
            {
                return ReturnBase.Fail(ex, _exceptionManager);
            }
        }
        //public async Task<ReturnBase> RegisterAsync(RegisterTenantUserDto user, string tenantName)
        //{
        //    ApplicationUser appUser = new ApplicationUser
        //    {
        //        ERPUserCodeId = user.ERPUserCodeId,
        //        TenantUserName = user.UserName,
        //        UserName = TenantHelper.GetNameWithTenant(tenantName, user.UserName),
        //        TenantName = tenantName,
        //        Active = user.Active,
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber,
        //        // Added by mazen for temporary fixing the bug
        //        Tenant_ID = user.TenantId
        //    };

        //    var result = await _userManager.CreateAsync(appUser, user.Password);
        //    if (result.Succeeded)
        //    {
        //        var roles = user.Roles.Select(r => TenantHelper.GetNameWithTenant(tenantName, r)).ToList();
        //        await _userManager.AddToRolesAsync(appUser, roles);
        //    }

        //    else
        //    {
        //        var errors = new List<ReturnBaseError>();
        //        foreach (var error in result.Errors)
        //        {
        //            errors.Add(new ReturnBaseError { ErrorCode = error.Code, ErrorMessage = error.Description });
        //        }
        //        return ReturnBase.Fail(errors);
        //    }

        //    return ReturnBase.Success();
        //}

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
                return ReturnBase.Fail(ex, _exceptionManager);
            }

        }

        public async Task<ReturnBase<TenantUserDto>> ValidateUserAsync(TenantLoginDto login)
        {
            var user = await _userManager.FindByNameAsync(TenantHelper.GetNameWithTenant(login.TenantName, login.UserName));
            var result = user != null && await _userManager.CheckPasswordAsync(user, login.Password);
            if (!result) return ReturnBase<TenantUserDto>.Fail();

            var tenantUser = new TenantUserDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                TenantUserName = user.TenantUserName,
                TenantName = user.TenantName,
                ERPUserCodeId = user.ERPUserCodeId
            };
            return ReturnBase<TenantUserDto>.Success(tenantUser);
        }


    }
}
