using Microsoft.AspNetCore.Identity;
using Shared.Domain.Contracts.EntityCommonData;
using Shared.Security.Domain.Models.Identity;

namespace Shared.Security.Infrastructure.Multitenant.IdentityModels
{
    public class ApplicationUser : IdentityUser, IApplicationUser, IDeletable, IActivable
    {
        public ApplicationUser() { }
        //public ApplicationUser(string? name, string tenantName,bool active=true) {
        //    TenantUserName = name;
        //    UserName = TenantHelper.GetNameWithTenant(tenantName, name);
        //    TenantName = tenantName;
        //    Active=active;
        //}
        public long? ERPUserCodeId { get; set; }
        public string? TenantUserName { get; set; }
        public string Tenant_ID { get; init; }
        public bool? Deleted { get; protected set; } = false;
        public bool? Active { get; set; }
        public string? TenantName { get; set; }

    }
}
