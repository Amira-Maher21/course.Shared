using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Contracts.EntityCommonData
{
    public interface IAuditable
    {
        public string In_User { get; set; }
        public DateTime In_Date { get; set; }
        public string? Mod_User { get; set; }
        public DateTime? Mod_Date { get; set; }
    }
}
