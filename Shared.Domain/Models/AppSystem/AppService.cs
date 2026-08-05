using Shared.Domain.Models.AppSystem;
using Shared.Domain.SystemConstants;
using Shared.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Models.AppSystem
{
    public class AppService : IRootEntity
    {
        [Key]
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string CurrentVersion { get; set; } =
            SystemDefaults.DefaultVersion;
        public string? ServiceDescription { get; set; }

        public virtual ICollection<AppServiceVersion>? Versions { get; set; }
    }


}
