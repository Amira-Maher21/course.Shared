
using  Shared.Domain.Models.Multitenant;
using Shared.Application.API;

namespace  Shared.Application.Multitenant
{
    public interface ITenantResolver
    {
        Tenant GetTenant();
        string GetTenantName();
        CommonUserData GetCommonUserData();
    }
}
