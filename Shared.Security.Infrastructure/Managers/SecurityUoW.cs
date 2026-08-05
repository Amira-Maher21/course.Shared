//using ECIT.ERP.Security.Infrastructure.Multitenant.CommandRepositories.TenantIdentity;
//using ECIT.ERP.Security.Infrastructure.Multitenant.CommandRepositories.Tenants;
//using ECIT.ERP.Security.Infrastructure.Multitenant.DataContext;
//using ECIT.ERP.Security.Infrastructure.Multitenant.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Shared.Infrastructure.UnitOfWorkBase;
using Shared.Application.Multitenant;
using Shared.Infrastructure.UnitOfWorkBase;
using Shared.Kernel.Exceptions;
using Shared.Security.Application.Contracts.CommandRepositories;
using Shared.Security.Application.Contracts.CommandRepositories.Multitenant;
using Shared.Security.Application.Contracts.Services.Clients.UserCodeClient;
using Shared.Security.Infrastructure.Multitenant.CommandRepositories.TenantIdentity;
using Shared.Security.Infrastructure.Multitenant.CommandRepositories.Tenants;
using Shared.Security.Infrastructure.Multitenant.DataContext;
using Shared.Security.Infrastructure.Multitenant.IdentityModels;
using Shared.Security.Infrastructure.System.IdentityModels;

namespace Shared.Security.Infrastructure.Managers
{
    public class SecurityUoW : UnitOfWorkBase, ISecurityUnitOfWork
    {
        private readonly Lazy<ITenantUserManager> _tenantUsers;
        //private readonly Lazy<ITenantRoleRepository> _tenantRoles;
        private readonly Lazy<ITenantRepository> _tenants;
        private readonly Lazy<ISystemUserManager> _systemUsers;
        private readonly Lazy<IERPUserCodeService> _erpUserCodeService;
        public SecurityUoW(UserManager<ApplicationUser> userManager, UserManager<SystemUser> userSystemManager, TenantDbContext context, IExceptionManager exceptionManager, ITenantResolver tenantResolver, IERPUserCodeService erpUserCodeService) : base(context, tenantResolver, exceptionManager)
        {
            // Initialize ERP service lazy first so it is available to other lazy factories
            _erpUserCodeService = new Lazy<IERPUserCodeService>(() => erpUserCodeService);

            // Use factories so objects are created lazily and after dependencies are initialized
            _tenantUsers = new Lazy<ITenantUserManager>(() => new TenantUserManager(userManager, context, this._exceptionManager, this.ERPUserCodeService));
            //this._tenantRoles = new Lazy<ITenantRoleRepository>(() => new TenantRoleRepository(userManager, dbContext));
            _tenants = new Lazy<ITenantRepository>(() => new TenantRepository(this._context, this._tenantResolver, this._exceptionManager));

            _systemUsers = new Lazy<ISystemUserManager>(() => new SystemUserManager(userSystemManager, context, this._exceptionManager));
        }
        public ITenantUserManager TenantUsers => _tenantUsers.Value;

        //public ITenantRoleRepository TenantRoles => throw new NotImplementedException();

        public ITenantRepository Tenants => _tenants.Value;

        public ISystemUserManager SystemUsers => _systemUsers.Value;
        public IERPUserCodeService ERPUserCodeService => _erpUserCodeService.Value;

    }
}
