using Shared.Domain.Contracts.EntityCommonData;
using Shared.Domain.Contracts.Multitenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Security.Domain.Models.Identity
{
    public interface IApplicationUser : ITenantEntity, IActivable
    {
        long? ERPUserCodeId { get; }
        string? UserName { get; }
        string? TenantUserName { get; }
        string? Email { get; }
        string? PhoneNumber { get; }
    }
}
