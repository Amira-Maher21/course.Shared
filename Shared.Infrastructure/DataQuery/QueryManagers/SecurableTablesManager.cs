

using Shared.Application.DataQuery;
using Shared.Infrastructure.DataContext;
using Shared.Infrastructure.DataQuery.QueryManagers;

namespace NDS.Shared.Infrastructure.DataQuery.QueryManagers
{
    internal class SecurableTablesManager : ISecurableTablesManager
    {

        private readonly DapperDbContext _dapper;
        private readonly Lazy<IEnumerable<SecurableTable>?> _securableTables;

        public SecurableTablesManager(DapperDbContext dapper)
        {
            _dapper = dapper;
            _securableTables = new Lazy<IEnumerable<SecurableTable>?>(getSecurableTables().Result);
        }

        private IEnumerable<SecurableTable>? tables
        {
            get { return _securableTables.Value; }
        }
        private async Task<IEnumerable<SecurableTable>?> getSecurableTables()
        {
            StaticQueriesStrings staticQueries = new StaticQueriesStrings();

            var queryResult = await _dapper.QueryList<SecurableField>(staticQueries.GetSecurableTablesQueryString());
            return queryResult.Result?.GroupBy(e => e.TableName)
                         .Select(ge => new SecurableTable { TableName = ge.Key, Fields = ge.Select(i => i.ColumnName).ToList() }).ToList();
        }

        public bool? IsSecurableTable(string tableName)
        {
            return tables?.Any(e => e.TableName == tableName.ToUpper());
        }

        public SecurableTable? GetSecurableTable(string tableName)
        {
            return tables?.FirstOrDefault(t => t.TableName == tableName.ToUpper());
        }

    }
}
