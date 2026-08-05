using Shared.Kernel.BaseReturnTypes;

namespace Shared.Application.DataQuery
{
    public interface ILookupQueryManager
    {
        Task<ReturnBase<IEnumerable<TResult>>> QueryLookup<TResult>();
    }
}
