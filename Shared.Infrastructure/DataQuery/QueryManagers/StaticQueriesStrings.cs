namespace Shared.Infrastructure.DataQuery.QueryManagers
{
    internal class StaticQueriesStrings
    {
        public string GetFiscalYearTablesQueryString()
        {
            var sqlQuery = " SELECT DISTINCT UPPER(obj.name) TableName " +
                           " FROM sys.columns INNER JOIN sys.SYSOBJECTS obj  ON columns.object_id = obj.id " +
                           " WHERE (UPPER(columns.name) = 'FISCAL_YEAR') AND ( obj.xtype = 'U' ) AND UPPER(obj.name) <> 'FISCAL_YEAR'";

            return sqlQuery;
        }

        public string GetSecurableTablesQueryString()
        {
            var sqlQuery = " SELECT UPPER(obj.name) TableName , UPPER(sys.columns.name) ColumnName   " +
                              " FROM sys.columns INNER JOIN sys.SYSOBJECTS obj  ON columns.object_id = obj.id " +
                              " WHERE ( UPPER(obj.name ) NOT LIKE '%SCHEMA' AND UPPER(obj.name ) NOT LIKE  '%SERIAL_%') " +
                              "      AND (UPPER(columns.name)  IN (SELECT UPPER([Security_Field]) FROM  [syst].[Security_Field] ))  " +
                              "      AND ( obj.xtype = 'U' )    ";

            return sqlQuery;
        }

        public string GetSecurityTablesQueryString()
        {
            var sqlQuery = " SELECT [Security_Table] [TableName], " +
                            "UPPER([Security_Table]) [NormalizedTableName], " +
                            " [Security_Field] [SecurityField], " +
                            " UPPER([Security_Field]) [NormalizedSecurityField]" +
                            " [Schema_Name] [SchemaName], " +
                            " FROM [syst].[Security_Field] ";

            return sqlQuery;
        }

        public string GetTablesMetadataQueryString(string filter)
        {
            var sqlQuery = " SELECT  UPPER(OBJECT_NAME(columns.object_id)) TableName," +
                        " columns.name ColumnName," +
                        " UPPER(columns.name) NormalizedColumnName," +
                        " columns.is_identity IsIdentity," +
                        " types.Name DataType," +
                        " columns.max_length MaxLength," +
                        " columns.precision Precision," +
                        " columns.scale Scale," +
                        " columns.is_nullable AllowNull," +
                        " CONVERT(BIT, ISNULL(i.is_primary_key, 0)) IsPrimaryKey " +
                        " FROM sys.columns INNER JOIN sys.types ON columns.user_type_id = types.user_type_id" +
                        " LEFT OUTER JOIN ( SELECT ic.object_id , ic.column_id ,MAX(CONVERT(INT, i.is_primary_key)) is_primary_key " +
                        " FROM sys.index_columns ic INNER JOIN sys.indexes i  ON ic.index_id = i.index_id AND ic.object_id = i.object_id " +
                        " GROUP BY ic.object_id , ic.column_id) i ON columns.object_id = i.object_id AND columns.column_id = i.column_id " +
                        " WHERE columns.object_id " + filter + " )";

            return sqlQuery;
        }
    }
}
