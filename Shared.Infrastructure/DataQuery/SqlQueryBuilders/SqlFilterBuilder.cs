using System.Text;

namespace NDS.Shared.Infrastructure.DataQuery.SqlQueryBuilders
{
    internal class SqlFilterBuilder
    {
        private readonly List<SqlFieldFilterData> _filtersData;

        public SqlFilterBuilder()
        {
            _filtersData = new List<SqlFieldFilterData>();
        }

        public void AddFilter(string tableAlias, string fieldName, object value,
                            SqlFilterOperator? @operator = null, string? parameterName = null)
        {
            if (@operator is null)
                @operator = SqlFilterOperator.Equal;

            _filtersData.Add(new SqlFieldFilterData
            {
                TableAliasName = tableAlias,
                FieldName = fieldName,
                Value = value,
                ParameterName = parameterName,
                Operator = @operator,

            });
        }

        public string GetFilterClause(ref List<ParameterData> parametersData)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE\t( 1 = 1 ) ");
            foreach (var filter in _filtersData)
            {
                builder.Append($"\n\t\tAND ({filter.GetFilterString()})");
                var parameterData = filter.GetFilterParameter();
                if (parameterData != null) { parametersData.Add(parameterData); }

            }

            return builder.ToString();
        }

    }

    internal class SqlFieldFilterData
    {
        public string? TableAliasName { get; set; }
        public string? FieldName { get; set; }
        public SqlFilterOperator Operator { get; set; } = SqlFilterOperator.Equal;
        public string? ParameterName { get; set; }
        public object? Value { get; set; }

        public string? GetFilterString()
        {
            return getFilterString(this);
        }

        public ParameterData? GetFilterParameter()
        {
            var tableName = string.IsNullOrEmpty(TableAliasName) ? string.Empty : $"[{TableAliasName.Trim()}].";
            var fieldName = !string.IsNullOrEmpty(FieldName) ? $"[{FieldName.Trim()}]" : throw new Exception("Invalid Column Name.");
            var parameter = ParameterName = getParameterName(ParameterName!, tableName, fieldName);


            if (Operator == SqlFilterOperator.IsNull || Operator == SqlFilterOperator.IsNotNull)
            {
                return null;
            }

            return new ParameterData
            {
                ParameterName = parameter,
                Value = SqlFilterOperator.IsSameOperator(Operator, SqlFilterOperator.Like) ? $"%{Value ?? ""}%" : Value!
            };
        }

        public static implicit operator string(SqlFieldFilterData filterData)
        {
            return getFilterString(filterData);
        }

        private static string getFilterString(SqlFieldFilterData filterData)
        {
            var tableName = string.IsNullOrEmpty(filterData.TableAliasName) ? string.Empty : $"[{filterData.TableAliasName.Trim()}].";
            var fieldName = !string.IsNullOrEmpty(filterData.FieldName) ? $"[{filterData.FieldName.Trim()}]" : throw new Exception("Invalid Column Name.");
            var parameter = filterData.ParameterName = getParameterName(filterData.ParameterName!, tableName, fieldName);

            if (filterData.Operator == SqlFilterOperator.IsNull || filterData.Operator == SqlFilterOperator.IsNotNull)
            {
                return $" {tableName}{fieldName} {filterData.Operator}";
            }


            return $" {tableName}{fieldName} {filterData.Operator} @{parameter}";

        }

        private static string getParameterName(string parameterName, string tableName, string fieldName)
        {
            var parameter = string.IsNullOrEmpty(parameterName) ? $"p_{tableName}_{fieldName}" : parameterName.Trim();
            parameter = parameter.Replace("[", "");
            parameter = parameter.Replace("]", "");
            parameter = parameter.Replace(".", "");

            return parameter;
        }
    }

    internal class ParameterData
    {
        public required string ParameterName { get; set; }
        public required object Value { get; set; }
    }

    internal class SqlFilterOperator
    {
        private string _symbol;
        private static string[] _symbols = { "=", "<>", ">", ">=", "<", "<=", "ISNULL", "ISNOTNULL", "LIKE", };
        private SqlFilterOperator(string symbol)
        {
            _symbol = symbol;
        }

        public static SqlFilterOperator Equal { get { return new SqlFilterOperator(" = "); } }
        public static SqlFilterOperator NotEqual { get { return new SqlFilterOperator(" <> "); } }
        public static SqlFilterOperator GreaterThan { get { return new SqlFilterOperator(" > "); } }
        public static SqlFilterOperator GreaterThanOrEqual { get { return new SqlFilterOperator(" >= "); } }
        public static SqlFilterOperator LessThan { get { return new SqlFilterOperator(" < "); } }
        public static SqlFilterOperator LessThanOrEqual { get { return new SqlFilterOperator(" <= "); } }
        public static SqlFilterOperator IsNull { get { return new SqlFilterOperator(" IS NULL "); } }
        public static SqlFilterOperator IsNotNull { get { return new SqlFilterOperator(" IS NOT NULL "); } }
        public static SqlFilterOperator Like { get { return new SqlFilterOperator(" LIKE "); } }

        public static bool IsSameOperator(SqlFilterOperator operatorOne, SqlFilterOperator operatorTwo)
        {
            return operatorOne._symbol == operatorTwo._symbol;
        }


        public static implicit operator string(SqlFilterOperator sqlOperator)
        {
            return sqlOperator._symbol;

        }

        public static implicit operator SqlFilterOperator(string symbol)
        {
            var @operator = symbol.Trim().Replace(" ", "").ToUpper();
            if (_symbols.Contains(@operator))
            {
                if (@operator == "ISNULL")
                {
                    @operator = "IS NULL";
                }
                else if (@operator == "ISNOTNULL")
                {
                    @operator = "IS NOT NULL";
                }
                return new SqlFilterOperator($" {@operator} ");
            }

            throw new Exception("Operator symbol is not in valid");
        }
        public override string ToString()
        {
            return _symbol;
        }
    }
}
