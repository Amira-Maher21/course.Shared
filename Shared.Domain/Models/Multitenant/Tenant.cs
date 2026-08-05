
 using Shared.Domain.Contracts;
using Shared.Domain.Contracts.EntityCommonData;
using Shared.Domain.Contracts.Multitenant;
using System.ComponentModel.DataAnnotations;

namespace  Shared.Domain.Models.Multitenant
{
    public class Tenant : ITenantEntity, IActivable, IDeletable, IRootEntity
    {

        [Key]
        public int Id { get; set; }
        public string Tenant_ID { get; set; }
        public string TenantType { get; set; }
        public string? TenantDescription { get; set; }
        public bool? Active { get; set; } = false;
        public bool? Deleted { get; set; } = false;
        public string? TenantName { get; set; }
        public virtual ICollection<TenantDb> TenantDbs { get; set; }

    }

    public class TenantType
    {
        private string _type;
        private TenantType(string type)
        {
            _type = type;
        }

        public static TenantType Default { get { return new TenantType("Default"); } }

        public static implicit operator string(TenantType tenantType)
        {
            return tenantType._type;
        }

        public override string ToString()
        {
            return _type;
        }
    }
}
