using Shared.Application.DataQuery;

namespace Shared.Application.DataQuery
{
    public interface ISqlQueryBuilder
    {
        Task<QueryStringData> GetQueryStringDataAsync(string? tableName, string fieldsData, SqlQueryOptions? queryOptions = null);
        Task<QueryStringData> GetQueryStringDataNoSecurityAsync(string? tableName, string fieldsData, SqlQueryOptions? queryOptions = null);

        Task<QueryStringData> GetQueryStringDataAsync(string? tableName, string fieldsData, IEnumerable<JoinTable> joinTables, SqlQueryOptions? queryOptions = null);
        Task<QueryStringData> GetQueryStringDataNoSecurityAsync(string? tableName, string fieldsData, IEnumerable<JoinTable> joinTables, SqlQueryOptions? queryOptions = null);
    }

    public enum NationalFieldsOption
    {
        OneLanguageOnly,
        TowLanguagesWithBrackets,
        TowLanguagesWithSeparator,
        TowFields,
        SecondIfNullWithBrackets,
        SecondIfNullWithoutBrackets
    }

    public class QueryStringData
    {
        public string? QueryString { get; init; }
        public IReadOnlyDictionary<string, object>? Parameters { get; init; }
    }

}
