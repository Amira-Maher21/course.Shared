using Shared.Security.Application.Contracts.Services.Multitenant;
using Shared.Security.Application.Contracts.Services.System;

namespace Shared.Security.Application.Contracts.Services
{
    public interface ISecurityServiceManager
    {
        ITenantService Tenants { get; }
        ITenantUserService TenantUsers { get; }
        ISystemUserService SystemUsers { get; }
    }
}
