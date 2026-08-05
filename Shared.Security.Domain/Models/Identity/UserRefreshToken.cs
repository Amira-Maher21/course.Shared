using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Security.Domain.Models.Identity
{
    public class UserRefreshToken
    {
        public string Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
