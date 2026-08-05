using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Kernel.Exceptions
{
    public interface IDatabaseExceptionManager
    {
        Task<string> GetErrorFromMessageAsync(string errorMessage);
    }
}
