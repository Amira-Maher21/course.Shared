using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Security.Application.Contracts.DTO.System.Identity
{
    public class RegisterSystemUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
    }
}
