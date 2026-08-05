using Shared.Domain.Contracts.Multitenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Security.Domain.Models.Identity
{
    public interface IApplicationRole : ITenantEntity
    {
        string? RoleName { get; }

    }
}
