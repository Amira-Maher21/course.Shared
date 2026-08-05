 using Shared.Domain.Contracts.Multitenant;
using Shared.Domain.Models.AppSystem;
using System.ComponentModel.DataAnnotations;

namespace  Shared.Domain.Models.Multitenant
{
    public class TenantDb : ITenantEntity
    {
        [Key]
        public string TenantDbId { get; set; }
        public string Tenant_ID { get; set; }
        public string ConnectionString { get; set; }
        public string? DbDescription { get; set; }
        public virtual AppService AppServices { get; set; }
    }
}
