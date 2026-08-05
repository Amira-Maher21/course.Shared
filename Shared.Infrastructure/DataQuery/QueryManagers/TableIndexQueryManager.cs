using Shared.Application.DataQuery;
using Shared.Kernel.BaseReturnTypes;

namespace NDS.Shared.Infrastructure.DataQuery.QueryManagers
{
    internal class TableIndexQueryManager : ITableIndexQueryManager
    {
        public Task<ReturnBase<IEnumerable<TResult>>> QueryIndex<TResult>()
        {
            throw new NotImplementedException();
        }
    }
}
