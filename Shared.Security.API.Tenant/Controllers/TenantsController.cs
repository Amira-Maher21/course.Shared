using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Security.API.Tenant.Controllers
{
    [Route("api/tenants/[action]")]
    [ApiController]
    public class TenantsController
    //: InspectionControllerBase
    {
    //    private readonly ISecurityServiceManager _serviceManager;

    //    public TenantsController(ISecurityServiceManager serviceManager)
    //    {
    //        _serviceManager = serviceManager;
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Register(RegisterTenantDto tenant)
    //    {
    //        var result = await _serviceManager.Tenants.RegisterTenant(tenant);
    //        if (result.Succeeded)
    //        {
    //            return Ok();
    //        }
    //        return StatusCode(500, result.Errors);
    //    }

    }
}
