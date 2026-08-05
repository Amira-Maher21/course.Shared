namespace Shared.Application.DataQuery
{
    public interface ITablesMetadataManager
    {
        Task<TableMetadata> GetTableMetadata(string tableName);
        Task<IEnumerable<TableMetadata>> GetMetadata(string[] tableNames);
    }

    public class TableMetadata
    {
        public string? TableName { get; set; }
        public IEnumerable<ColumnMetadata>? Columns { get; set; }

        public bool HasField(string fieldName)
        {
            return Columns != null ? Columns.Any(c => c.NormalizedColumnName == fieldName.ToUpper()) : false;
        }
    }

    public class TableColumnMetadata : ColumnMetadata
    {
        public string? TableName { get; set; }
    }

    public class ColumnMetadata
    {
        public string? ColumnName { get; set; }
        public string? NormalizedColumnName { get; set; }
        public bool IsIdentity { get; set; }
        public string? DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool AllowNull { get; set; }
        public short MaxLength { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
    }
}
