using Shared.Application.DataQuery;
using Shared.Infrastructure.DataContext;
using Shared.Infrastructure.DataQuery.QueryManagers;

namespace NDS.Shared.Infrastructure.DataQuery.QueryManagers
{
    internal class SecurityTablesManager : ISecurityTablesManager
    {
        private readonly DapperDbContext _dapper;
        private readonly Lazy<IEnumerable<SecurityTable>> _securityTables;


        public SecurityTablesManager(DapperDbContext dapper)
        {
            _dapper = dapper;
            _securityTables = new Lazy<IEnumerable<SecurityTable>>(GetSecurityTables().Result);
        }

        private IEnumerable<SecurityTable> tables
        {
            get { return _securityTables.Value; }
        }
        private async Task<IEnumerable<SecurityTable>> GetSecurityTables()
        {
            StaticQueriesStrings staticQueries = new StaticQueriesStrings();
            var queryResult = await _dapper.QueryList<SecurityTable>(staticQueries.GetSecurityTablesQueryString());
            return queryResult.Result;
        }

        public bool IsSecurityTable(string tableName)
        {
            return tables.Any(e => e.NormalizedTableName == tableName.ToUpper());
        }

        public SecurityTable? GetSecurityTable(string tableName)
        {
            return tables.FirstOrDefault(t => t.NormalizedTableName == tableName.ToUpper());
        }

        public SecurityTable? GetSecurityTableByField(string fieldName)
        {
            return tables.SingleOrDefault(e => e.NormalizedSecurityField == fieldName.ToUpper());
        }


    }

}
