 using  Shared.Security.Application.Contracts.CommandRepositories.Multitenant;
using Shared.Application.UnitOfWorkBase;
using Shared.Security.Application.Contracts.CommandRepositories.Multitenant;

namespace  Shared.Security.Application.Contracts.CommandRepositories
{
    public interface ISecurityUnitOfWork : IUnitOfWorkBase
    {
        ITenantUserManager TenantUsers { get; }
        //ITenantRoleRepository TenantRoles { get; }
        ITenantRepository Tenants { get; }
        ISystemUserManager SystemUsers { get; }
    }
}
