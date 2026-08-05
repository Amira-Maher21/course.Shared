using Shared.Application.DataQuery;
using Shared.Kernel.BaseReturnTypes;

namespace NDS.Shared.Infrastructure.DataQuery.QueryManagers
{
    internal class LookupQueryManager : ILookupQueryManager
    {
        public LookupQueryManager() { }

        public async Task<ReturnBase<IEnumerable<TResult>>> QueryLookup<TResult>()
        {
            return null;
        }
    }
}
