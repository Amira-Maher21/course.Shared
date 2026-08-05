using Shared.Application.DataQuery;
using Shared.Infrastructure.DataContext;

namespace Shared.Infrastructure.DataQuery.QueryManagers
{
    internal class FiscalYearTablesManager : IFiscalYearTablesManager
    {
        private readonly DapperDbContext _dapper;
        private readonly Lazy<IEnumerable<string>> _fiscalYearTables;

        public FiscalYearTablesManager(DapperDbContext dapper)
        {
            _dapper = dapper;
            _fiscalYearTables = new Lazy<IEnumerable<string>>(GetFiscalYearTables().Result);
        }

        private IEnumerable<string> tables
        {
            get { return _fiscalYearTables.Value; }
        }

        private async Task<IEnumerable<string>> GetFiscalYearTables()
        {
            StaticQueriesStrings staticQueries = new StaticQueriesStrings();
            try
            {
                var queryResult = await _dapper.QueryList<string>(staticQueries.GetFiscalYearTablesQueryString());
                return queryResult.Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public bool IsFiscalYearTable(string tableName)
        {
            return tables.Any(t => t == tableName.ToUpper());
        }
    }
}
