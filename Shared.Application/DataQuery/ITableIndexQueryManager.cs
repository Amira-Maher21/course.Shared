using Shared.Kernel.BaseReturnTypes;

namespace Shared.Application.DataQuery
{
    public interface ITableIndexQueryManager
    {
        Task<ReturnBase<IEnumerable<TResult>>> QueryIndex<TResult>();
    }
}
