namespace Shared.Domain.SystemConstants
{
    public static class TenantHelper
    {
        public static string GetNameWithTenant(string tenantName, string name)
        {
            return $"{tenantName}@@{name}";
        }
    }
}
