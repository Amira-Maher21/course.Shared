using Microsoft.AspNetCore.Identity;
using   Shared.Domain.Contracts.EntityCommonData;
using  Shared.Domain.SystemConstants;
using  Shared.Security.Domain.Models.Identity;

namespace  Shared.Security.Infrastructure.Multitenant.IdentityModels
{
    public class ApplicationRole : IdentityRole, IApplicationRole, IDeletable, IActivable
    {
        public ApplicationRole()
        {

        }
        public ApplicationRole(string name, string tenantName, bool active)
        {
            RoleName = name;
            Tenant_ID = tenantName;
            Active = active;
            Name = TenantHelper.GetNameWithTenant(tenantName, name);
        }

        internal void SetDelete(bool deleted)
        {
            Deleted = deleted;
        }

        internal void SetActive(bool active) { Active = active; }

        internal void RenameRole(string name)
        {
            RoleName = name;
            Name = TenantHelper.GetNameWithTenant(Tenant_ID, name);
        }
        public string? RoleName { get; protected set; }

        public string Tenant_ID { get; protected set; }
        public bool? Deleted { get; protected set; }
        public bool? Active { get; protected set; }


    }
}
