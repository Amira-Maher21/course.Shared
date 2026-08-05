using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Contracts.EntityCommonData
{
    public interface IActivable
    {
        bool? Active { get; }
    }
}
