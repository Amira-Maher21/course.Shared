using NDS.Shared.Infrastructure.DataQuery.QueryManagers;
using Shared.Application.DataQuery;
using Shared.Infrastructure.DataContext;
using Shared.Infrastructure.DataQuery.QueryManagers;

namespace Shared.Infrastructure.DataQuery
{
    public class DataQueryManager : IDataQueryManager
    {
        private readonly Lazy<IFiscalYearTablesManager> _fiscalYearManager;
        private readonly Lazy<ISecurableTablesManager> _securableTablesManager;
        private readonly Lazy<ISecurityTablesManager> _securityTablesManager;
        private readonly Lazy<ILookupQueryManager> _lookupQueryManager;
        private readonly Lazy<ITableIndexQueryManager> _tableIndexQueryManager;
        private readonly Lazy<ITablesMetadataManager> _tableMetadataManager;
        private readonly DapperDbContext _dapper;

        public DataQueryManager(DapperDbContext dapper)
        {
            _dapper = dapper;
            _fiscalYearManager = new Lazy<IFiscalYearTablesManager>(new FiscalYearTablesManager(dapper));
            _securableTablesManager = new Lazy<ISecurableTablesManager>(new SecurableTablesManager(dapper));
            _securityTablesManager = new Lazy<ISecurityTablesManager>(new SecurityTablesManager(dapper));
            _lookupQueryManager = new Lazy<ILookupQueryManager>(new LookupQueryManager());
            _tableIndexQueryManager = new Lazy<ITableIndexQueryManager>(new TableIndexQueryManager());
            _tableMetadataManager = new Lazy<ITablesMetadataManager>(new TablesMetadataManager(dapper));
        }

        public IFiscalYearTablesManager FiscalYearManager { get { return _fiscalYearManager.Value; } }
        public ISecurableTablesManager SecurableTablesManager { get { return _securableTablesManager.Value; } }
        public ISecurityTablesManager SecurityTablesManager { get { return _securityTablesManager.Value; } }
        public ILookupQueryManager LookupQueryManager { get { return _lookupQueryManager.Value; } }
        public ITableIndexQueryManager TableIndexQueryManager { get { return _tableIndexQueryManager.Value; } }
        public ITablesMetadataManager TableMetadataManager { get { return _tableMetadataManager.Value; } }
    }
}
