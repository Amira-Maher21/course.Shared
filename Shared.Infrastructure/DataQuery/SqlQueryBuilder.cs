using Shared.Application.DataQuery;
using Shared.Application.Multitenant;
using NDS.Shared.Infrastructure.DataQuery.SqlQueryBuilders;
using System.Text;

namespace  Shared.Infrastructure.DataQuery
{
    public class SqlQueryBuilder : ISqlQueryBuilder
    {
        private readonly IDataQueryManager _queryManager;
        private readonly ITenantResolver _tenantResolver;
        private readonly SqlQuerySelectFieldsBuilder _selectBuilder;
        private readonly SqlQueryJoinBuilder _joinBuilder;
        private readonly SqlQueryJoinsBuilder _joinsBuilder;
        private readonly SqlFilterBuilder _filterBuilder;
        private readonly SqlQueryOrderBuilder _orderBuilder;
        private readonly SqlQueryPagingBuilder _pagingBuilder;

        public SqlQueryBuilder(IDataQueryManager queryManager, ITenantResolver tenantResolver)
        {
            _queryManager = queryManager;
            _tenantResolver = tenantResolver;
            _selectBuilder = new SqlQuerySelectFieldsBuilder();
            _joinBuilder = new SqlQueryJoinBuilder();
            _joinsBuilder = new SqlQueryJoinsBuilder();
            _filterBuilder = new SqlFilterBuilder();
            _orderBuilder = new SqlQueryOrderBuilder();
            _pagingBuilder = new SqlQueryPagingBuilder();
        }

        public async Task<QueryStringData> GetQueryStringDataAsync(string? tableName, string fieldsData, SqlQueryOptions? queryOptions = null)
        {
            return await SingleTableQuery(tableName, fieldsData, false, queryOptions);
        }

        public async Task<QueryStringData> GetQueryStringDataNoSecurityAsync(string? tableName, string fieldsData, SqlQueryOptions? queryOptions = null)
        {
            return await SingleTableQuery(tableName, fieldsData, true, queryOptions);
        }


        public async Task<QueryStringData> GetQueryStringDataAsync(string? tableName, string fieldsData, IEnumerable<JoinTable> joinTables, SqlQueryOptions? queryOptions = null)
        {

            return await GetMultiTablesQueryStringDataAsync(tableName, fieldsData, joinTables, true, queryOptions);

        }

        public async Task<QueryStringData> GetQueryStringDataNoSecurityAsync(string? tableName, string fieldsData, IEnumerable<JoinTable> joinTables, SqlQueryOptions? queryOptions = null)
        {
            return await GetMultiTablesQueryStringDataAsync(tableName, fieldsData, joinTables, false, queryOptions);
        }


        private async Task<QueryStringData> GetMultiTablesQueryStringDataAsync(string tableName, string fieldsData, IEnumerable<JoinTable> joinTables, bool noSecurity, SqlQueryOptions? queryOptions = null)

        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrWhiteSpace(fieldsData))
                throw new ArgumentNullException(nameof(fieldsData));

            if (joinTables == null || !joinTables.Any())
                throw new ArgumentNullException(nameof(joinTables));

            const string mainTableAlias = "T1";
            int tableAliasNo = 2;

            var userData = _tenantResolver.GetCommonUserData();

            bool forceOrderBy = false;
            bool paging = false;
            int take = 0;
            int skip = 0;

            var securableTable = noSecurity
                ? null
                : _queryManager.SecurableTablesManager.GetSecurableTable(tableName);

            var isFiscalYearTable = _queryManager.FiscalYearManager.IsFiscalYearTable(tableName);

            var tableMetaData = await _queryManager
                .TableMetadataManager
                .GetTableMetadata(tableName);

            // ---------------- SECURITY JOINS ----------------

            if (securableTable != null)
            {
                foreach (var fieldName in securableTable.Fields!)
                {
                    var securityTable =
                        _queryManager.SecurityTablesManager.GetSecurityTableByField(fieldName!);

                    if (securityTable == null)
                        continue;

                    var securityAlias = $"T{tableAliasNo++}";

                    _joinsBuilder.AddInnerJoin(mainTableAlias, securityTable.SecurityField!, securityTable.TableNameWithSchema!, securityAlias);

                    _filterBuilder.AddFilter(securityAlias, UserIdFieldName, userData.UserName!);
                }
            }

            // FISCAL YEAR FILTER 

            if (isFiscalYearTable)
            {
                _filterBuilder.AddFilter(mainTableAlias, FiscalYearFieldName, userData.FiscalYear!);
            }

            // MAIN TABLE SELECT 

            _selectBuilder.AddFields(fieldsData, mainTableAlias, NationalFieldsOption.SecondIfNullWithBrackets);

            // ---------------- JOIN TABLES ----------------

            foreach (var joinTable in joinTables)
            {
                var joinAlias = $"T{tableAliasNo++}";

                // ---- SELECT FIELDS FROM JOIN TABLE ----

                var joinSelectBuilder = new StringBuilder();

                foreach (var col in joinTable.SelectColumns)
                {
                    joinSelectBuilder
                        .Append(' ')
                        .Append(col.ColumnName)
                        .Append(' ')
                        .Append(col.ColumnAliasName)
                        .Append(',');
                }

                if (joinSelectBuilder.Length > 0)
                    joinSelectBuilder.Length--; // trim last comma

                _selectBuilder.AddFields(joinSelectBuilder.ToString(), joinAlias);
                // ---- JOIN CONDITIONS ----

                var joinColumns = joinTable.JoinColumns
                    .Select(j => new[]
                    {
                j.MainTableColumnName,
                j.JoinTableColumnName
                    })
                    .ToList();

                _joinsBuilder.AddLeftOuterJoin(mainTableAlias, joinTable.TableNameWithSchema, joinAlias, joinColumns);
            }

            // ---------------- FILTERS ----------------

            if (queryOptions?.Filters != null)
            {
                foreach (var filter in queryOptions.Filters)
                {
                    if (!tableMetaData.HasField(filter[0]))
                        throw new Exception($"Filter field {filter[0]} is not in table metadata");

                    _filterBuilder.AddFilter(mainTableAlias, filter[0], filter[2], filter[1]);
                }
            }
            // ---------------- SORTING ----------------

            if (queryOptions?.Sorts != null)
            {
                foreach (var sort in queryOptions.Sorts)
                {
                    if (string.IsNullOrWhiteSpace(sort.FieldName))
                        continue;

                    if (!tableMetaData.HasField(sort.FieldName))
                        throw new Exception($"Sort field {sort.FieldName} is not in table metadata");

                    _orderBuilder.AddSort(mainTableAlias, sort.FieldName, sort.IsAscending ?? true, sort.IsNullFirst);
                }
            }

            // ---------------- PAGING ----------------

            if (queryOptions?.Take > 0)
            {
                paging = true;

                if (!_orderBuilder.HasSorts())
                    forceOrderBy = true;

                take = queryOptions.Take.Value;
                skip = queryOptions.Skip;
            }

            // ---------------- BUILD SQL ----------------

            var parametersData = new List<ParameterData>();

            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine(_selectBuilder.GetSelectClause(tableName, mainTableAlias));

            sqlBuilder.AppendLine(_joinsBuilder.GetJoinClauses());

            sqlBuilder.AppendLine(_filterBuilder.GetFilterClause(ref parametersData));

            sqlBuilder.AppendLine(_orderBuilder.GetOrderByClause(forceOrderBy));

            if (paging)
            {
                sqlBuilder.AppendLine(
                    _pagingBuilder.GetPagingClause(skip, take));
            }

            // ---------------- PARAMETERS ----------------

            var parameters = parametersData.ToDictionary(
                p => p.ParameterName,
                p => p.Value!
            );

            return new QueryStringData
            {
                QueryString = sqlBuilder.ToString(),
                Parameters = parameters
            };
        }

        private async Task<QueryStringData> SingleTableQuery(string? tableName, string fieldsData, bool noSecurity, SqlQueryOptions? queryOptions = null)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));
            if (string.IsNullOrEmpty(fieldsData))
                throw new ArgumentNullException(nameof(fieldsData));

            int tableAliasNo = 2;
            string mainTableAlias = "T1";

            var userData = _tenantResolver.GetCommonUserData();

            var forceOrderBy = false;

            var paging = false;
            var take = 0;
            var skip = 0;

            var securableTable = noSecurity ? null : _queryManager.SecurableTablesManager.GetSecurableTable(tableName);
            var isFiscalYearTable = _queryManager.FiscalYearManager.IsFiscalYearTable(tableName);
            var tableMetaData = await _queryManager.TableMetadataManager.GetTableMetadata(tableName);

            List<SecurityTable> securityTables = new List<SecurityTable>();

            if (securableTable != null)
            {
                foreach (var fieldName in securableTable.Fields!)
                {
                    var table = _queryManager.SecurityTablesManager.GetSecurityTableByField(fieldName!);
                    if (table != null) securityTables.Add(table);

                }

                foreach (var table in securityTables)
                {
                    var joinTableAlias = $"T{tableAliasNo++}";
                    _joinBuilder.AddInnerJoin(mainTableAlias, table.SecurityField!, table.TableNameWithSchema!, joinTableAlias);
                    _filterBuilder.AddFilter(joinTableAlias, UserIdFieldName, userData.UserName!);

                }
            }



            if (isFiscalYearTable)
            {
                _filterBuilder.AddFilter(mainTableAlias, FiscalYearFieldName, userData.FiscalYear!);
            }

            _selectBuilder.AddFields(fieldsData, mainTableAlias, NationalFieldsOption.SecondIfNullWithBrackets);


            if (queryOptions != null)
            {
                if (queryOptions.Filters != null && queryOptions.Filters.Any())
                {
                    foreach (var field in queryOptions.Filters)
                    {
                        if (!tableMetaData.HasField(field[0]))
                        {
                            throw new Exception($"Filter field {field[0]} is not in the table fields.!!");
                        }
                        _filterBuilder.AddFilter(mainTableAlias, field[0], field[2], field[1]);
                    }
                }

                if (queryOptions.Sorts != null && queryOptions.Sorts.Any())
                {
                    foreach (var sort in queryOptions.Sorts)
                    {
                        if (!string.IsNullOrEmpty(sort.FieldName))
                        {
                            if (!tableMetaData.HasField(sort.FieldName))
                            {
                                throw new Exception($"Filter field {sort.FieldName} is not in the table fields.!!");
                            }
                            _orderBuilder.AddSort(mainTableAlias, sort.FieldName, sort.IsAscending ?? true, sort.IsNullFirst);
                        }
                    }
                }

                if (queryOptions.Take != null && queryOptions.Take > 0)
                {
                    paging = true;
                    if (!_orderBuilder.HasSorts())
                    {
                        forceOrderBy = true;
                    }
                    take = queryOptions.Take.Value;
                    skip = queryOptions.Skip;

                }
            }


            List<ParameterData> parametersData = new List<ParameterData>();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(_selectBuilder.GetSelectClause(tableName, mainTableAlias));
            builder.AppendLine(_joinBuilder.GetJoinClauses());
            builder.AppendLine(_filterBuilder.GetFilterClause(ref parametersData));
            builder.AppendLine(_orderBuilder.GetOrderByClause(forceOrderBy));
            if (paging)
            {
                builder.AppendLine(_pagingBuilder.GetPagingClause(skip, take));
            }



            Dictionary<string, object> parameters = new Dictionary<string, object>();
            foreach (var parameter in parametersData)
            {
                parameters.Add(parameter.ParameterName, parameter.Value!);
            }

            return new QueryStringData
            {
                QueryString = builder.ToString(),
                Parameters = parameters
            };

        }

        private static string UserIdFieldName
        {
            get { return "User_ID"; }
        }

        private static string FiscalYearFieldName
        {
            get { return "Fiscal_Year"; }
        }
    }
}
