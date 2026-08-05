using Microsoft.EntityFrameworkCore;
using Shared.Security.Infrastructure.RepositoryBase;
using Shared.Application.Multitenant;
using Shared.Domain.Models.Multitenant;
using Shared.Kernel.Exceptions;
using Shared.Security.Application.Contracts.CommandRepositories.Multitenant;
using Shared.Security.Infrastructure.RepositoryBase;

namespace Shared.Security.Infrastructure.Multitenant.CommandRepositories.Tenants
{
    internal class TenantRepository : SecurityCommandRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(DbContext context, ITenantResolver tenantResolver, IExceptionManager exceptionManager) : base(context, tenantResolver, exceptionManager)
        {
        }
    }
}
