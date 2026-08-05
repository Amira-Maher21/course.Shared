namespace Shared.Application.DataQuery
{
    public interface ISecurableTablesManager
    {
        bool? IsSecurableTable(string tableName);
        SecurableTable? GetSecurableTable(string tableName);
    }

    public class SecurableTable
    {
        public string? TableName { get; set; }
        public List<string?>? Fields { get; set; }
    }

    public class SecurableField
    {
        public string? TableName { get; set; }
        public string? ColumnName { get; set; }
    }
}
