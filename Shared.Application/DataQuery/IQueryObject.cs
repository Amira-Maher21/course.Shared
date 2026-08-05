using  Shared.Kernel.BaseReturnTypes;

namespace  Shared.Application.DataQuery
{
    public interface IQueryObject<TResult>
    {
        Task<ReturnBase<IEnumerable<TResult>>> Query(SqlQueryOptions queryOptions);
        Task<ReturnBase<IEnumerable<TResult>>> Query(SqlQueryOptions queryOptions, object[] functionParameters);
    }

    public interface IScalarQueryObject<TResult>
    {
        Task<ReturnBase<TResult>> Query(SqlQueryOptions queryOptions);
    }


}
