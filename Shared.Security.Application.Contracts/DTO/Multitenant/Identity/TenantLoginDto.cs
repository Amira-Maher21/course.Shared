namespace  Shared.Security.Application.Contracts.DTO.Multitenant.Identity
{
    public class TenantLoginDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? TenantName { get; set; }
    }
}
