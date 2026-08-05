using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Security.API.Tenant.Controllers
{
    
    public class TenantUsersController : ControllerBase
    {
        //public class TenantUsersController : InspectionControllerBase
        //{
        //    private readonly ITenantUserService _tenantUserService;
        //    private readonly ITenantResolver _tenantResolver;

        //    public TenantUsersController(ISecurityServiceManager serviceManager, ITenantResolver tenantResolver)
        //    {
        //        _tenantUserService = serviceManager.TenantUsers;
        //        _tenantResolver = tenantResolver;
        //    }

        //    [HttpPost]
        //    public async Task<IActionResult> Register(RegisterTenantUserDto user)
        //    {
        //        //string tenantName = _tenantResolver.GetTenantName();
        //        var registerResult = await _tenantUserService.RegisterAsync(user);
        //        return Ok();
        //    }

        //    [HttpPost]
        //    public async Task<IActionResult> Login(TenantLoginDto login)
        //    {
        //        var loginResult = await _tenantUserService.LoginAsync(login);
        //        if (loginResult.Succeeded)
        //        {
        //            return Ok(loginResult.Result);
        //        }
        //        return StatusCode(500, loginResult.Errors);
        //    }
        //}

    }
}
