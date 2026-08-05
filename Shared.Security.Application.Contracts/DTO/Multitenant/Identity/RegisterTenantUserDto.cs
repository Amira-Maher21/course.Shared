using  Shared.Security.Application.Contracts.DTO.Clients.UserCodeClientDTOs;
using  Shared.Security.Application.Contracts.DTO.Clients.UserCodeClientDTOs;

namespace Shared.Security.Application.Contracts.DTO.Multitenant.Identity
{
    public class RegisterTenantUserDto
    {
        //public long? ERPUserCodeId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string TenantName { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public string? Password { get; set; }
        public List<string>? Roles { get; set; }
        public bool? Active { get; set; }

        // ERP settings
        public ERPUserOptions? ERPOptions { get; set; }
    }
}