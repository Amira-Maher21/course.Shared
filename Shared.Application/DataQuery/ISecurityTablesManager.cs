namespace Shared.Application.DataQuery
{
    public interface ISecurityTablesManager
    {
        bool IsSecurityTable(string tableName);
        SecurityTable? GetSecurityTable(string tableName);
        SecurityTable? GetSecurityTableByField(string fieldName);
    }

    public class SecurityTable
    {
        public string? TableName { get; set; }
        public string? NormalizedTableName { get; set; }
        public string? SecurityField { get; set; }
        public string? NormalizedSecurityField { get; set; }

        public string? SchemaName { get; set; }

        public string? TableNameWithSchema
        {
            get
            {
                string schema;
                if (!string.IsNullOrEmpty(SchemaName))
                {
                    schema = $"[{SchemaName}].";
                }
                else
                {
                    schema = string.Empty;
                }
                return $"{schema}[{TableName}_dUser]";
            }
        }


    }
}
