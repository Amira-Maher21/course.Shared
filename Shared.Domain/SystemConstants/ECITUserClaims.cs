namespace Shared.Domain.SystemConstants
{
    public static class InspectionUserClaims
    {
        public static string ERPUserCodeId { get { return $"{multitenantClaimPrefix}ERPUserCodeId"; } }
        public static string TenantName { get { return $"{multitenantClaimPrefix}TenantName"; } }
        public static string TenantUserName { get { return $"{multitenantClaimPrefix}TenantUserName"; } }
        private static string multitenantClaimPrefix { get { return "InspectionMultitenantClaims"; } }
        private static string systemClaimPrefix { get { return "InspectionSystemClaims"; } }
    }
}