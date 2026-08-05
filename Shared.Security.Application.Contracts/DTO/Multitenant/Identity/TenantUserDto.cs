namespace Shared.Security.Application.Contracts.DTO.Multitenant.Identity
{
    public class TenantUserDto
    {
        public long? ERPUserCodeId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? TenantUserName { get; set; }
        public string? TenantName { get; set; }
    }
}
