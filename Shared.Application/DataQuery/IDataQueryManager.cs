 
namespace Shared.Application.DataQuery
{
    public interface IDataQueryManager
    {
        IFiscalYearTablesManager FiscalYearManager { get; }
        ISecurableTablesManager SecurableTablesManager { get; }
        ISecurityTablesManager SecurityTablesManager { get; }
        ILookupQueryManager LookupQueryManager { get; }
        ITableIndexQueryManager TableIndexQueryManager { get; }
        ITablesMetadataManager TableMetadataManager { get; }

    }
}
