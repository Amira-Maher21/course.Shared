using Shared.Application.DataQuery;
using Shared.Infrastructure.DataContext;
using Shared.Infrastructure.DataQuery.QueryManagers;
using System.Text;

namespace NDS.Shared.Infrastructure.DataQuery.QueryManagers
{

    internal class TablesMetadataManager : ITablesMetadataManager
    {
        private readonly DapperDbContext _dapper;
        private readonly Lazy<HashSet<TableMetadata>> _tablesMetadata;

        public TablesMetadataManager(DapperDbContext dapper)
        {
            _dapper = dapper;
            _tablesMetadata = new Lazy<HashSet<TableMetadata>>(new HashSet<TableMetadata>());


        }
        public async Task<TableMetadata> GetTableMetadata(string tableName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("tableName", tableName);

            StaticQueriesStrings staticQueries = new StaticQueriesStrings();
            string query = staticQueries.GetTablesMetadataQueryString("= ( OBJECT_ID(@tableName)");



            var queryResult = await _dapper.QueryList<TableColumnMetadata>(query, parameters);

            var columns = queryResult.Result.Select(c => new ColumnMetadata
            {
                AllowNull = c.AllowNull,
                ColumnName = c.ColumnName,
                NormalizedColumnName = c.NormalizedColumnName,
                DataType = c.DataType,
                IsIdentity = c.IsIdentity,
                IsPrimaryKey = c.IsPrimaryKey,
                MaxLength = c.MaxLength,
                Precision = c.Precision,
                Scale = c.Scale,

            });

            var table = new TableMetadata
            {
                TableName = tableName.ToUpper(),
                Columns = columns,
            };
            _tables.Add(table);
            return table;
        }

        public async Task<IEnumerable<TableMetadata>> GetMetadata(string[] tablesNames)
        {
            var allNames = tablesNames.Select(t => t.ToUpper()).ToList();
            List<string> names = new List<string>();
            if (tablesNames != null && tablesNames.Any())
            {
                var loadedTables = _tables.Where(t => allNames.Contains(t.TableName)).ToList();
                var unloadedTableNames = allNames.Where(n => !loadedTables.Select(t => t.TableName).Contains(n));
                if (unloadedTableNames.Any())
                {
                    loadedTables.AddRange(await GetTablesMetadata(unloadedTableNames.ToArray()));
                }
                return loadedTables;
            }
            else
            {
                throw new Exception("Tables names list is empty or null.!!");
            }

        }
        private HashSet<TableMetadata> _tables
        {
            get { return _tablesMetadata.Value; }
        }
        private async Task<IEnumerable<TableMetadata>> GetTablesMetadata(string[] tableNames)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder builder = new StringBuilder(" IN (  OBJECT_ID(N'') ");
            for (int i = 0; i < tableNames.Count(); i++)
            {
                parameters.Add($"table_{i}", tableNames[i]);
                builder.Append($" , OBJECT_ID(@table_{i}) ");
            }

            StaticQueriesStrings staticQueries = new StaticQueriesStrings();

            string query = staticQueries.GetTablesMetadataQueryString(builder.ToString());


            var queryResult = await _dapper.QueryList<TableColumnMetadata>(query, parameters);
            var queryResultGroup = queryResult.Result.GroupBy(t => t.TableName);
            var tables = new List<TableMetadata>();
            foreach (var table in queryResultGroup)
            {
                tables.Add(new TableMetadata
                {
                    TableName = table.Key,
                    Columns = table.Select(c => new ColumnMetadata
                    {
                        AllowNull = c.AllowNull,
                        ColumnName = c.ColumnName,
                        NormalizedColumnName = c.NormalizedColumnName,
                        DataType = c.DataType,
                        IsIdentity = c.IsIdentity,
                        IsPrimaryKey = c.IsPrimaryKey,
                        MaxLength = c.MaxLength,
                        Precision = c.Precision,
                        Scale = c.Scale,

                    })
                });
            }

            foreach (var table in tables)
            {
                _tables.Add(table);
            }
            return tables;

        }
    }
}
