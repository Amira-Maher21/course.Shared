namespace  Shared.Security.Application.Contracts.DTO.Multitenant.Tenants
{
    public class RegisterTenantDto
    {
        public string? TenantName { get; set; }
        public string? TenantDescription { get; set; }
        public string TenantId { get; set; }
    }
}
