using System.Text;

namespace NDS.Shared.Infrastructure.DataQuery.SqlQueryBuilders
{
    internal class SqlQueryOrderBuilder
    {
        private readonly List<SqlQuerySort> _sorts;

        public SqlQueryOrderBuilder()
        {
            _sorts = new List<SqlQuerySort>();
        }

        public bool HasSorts()
        {
            return _sorts.Any();
        }

        public void AddSort(string tableAlias, string fieldName, bool isAscending, bool? nullFirst)
        {
            _sorts.Add(new SqlQuerySort(tableAlias, fieldName, new SqlQuerySort.SqlSortType(isAscending, nullFirst)));

        }

        public string GetOrderByClause(bool forceOrderBy = true)
        {
            if (forceOrderBy == false && !_sorts.Any())
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(" ORDER BY ");
            if (forceOrderBy == true && !_sorts.Any())
            {
                builder.AppendLine("\t(SELECT 1)   , ");
            }
            foreach (var sort in _sorts)
            {
                builder.Append($"\n\t\t{sort}   , ");
            }

            builder.Remove(builder.Length - 4, 4);
            return builder.ToString();
        }
    }

    public class SqlQuerySort
    {

        public SqlQuerySort(string tableAlias, string fieldName, SqlSortType? sort = null)
        {
            if (tableAlias is null) throw new ArgumentNullException(nameof(tableAlias));
            if (fieldName is null) throw new ArgumentNullException(nameof(fieldName));

            TableAlias = tableAlias;
            FieldName = fieldName;
            SortType = sort;
        }

        public string TableAlias { get; init; }
        public string FieldName { get; init; }
        public SqlSortType? SortType { get; init; }

        private string FieldFullName { get { return $"[{TableAlias}].[{FieldName}]"; } }

        public static implicit operator string(SqlQuerySort fieldSort)
        {
            return fieldSort.ToString();
        }

        public override string ToString()
        {
            return getSortStatement();
        }

        private string getSortStatement()
        {
            if (SortType is null)
            {
                return $" {FieldFullName} ";
            }

            var sortType = SortType.GetSortType();

            if (SortType == Ascending || SortType == Descending)
            {
                return $" {FieldFullName} {sortType} ";
            }

            var nullIndex = SortType.GetNullsIndex();
            var fieldIndex = 1 - nullIndex;
            return $" CASE WHEN {FieldFullName} IS NULL THEN {nullIndex} ELSE  {fieldIndex} END  {sortType}, {FieldFullName} ";

        }

        public static SqlSortType Ascending { get { return new SqlSortType(true); } }
        public static SqlSortType AscendingNullsFirst { get { return new SqlSortType(true, true); } }
        public static SqlSortType AscendingNullsLast { get { return new SqlSortType(true, false); } }
        public static SqlSortType Descending { get { return new SqlSortType(false); } }
        public static SqlSortType DescendingNullsFirst { get { return new SqlSortType(false, true); } }
        public static SqlSortType DescendingNullsLast { get { return new SqlSortType(false, false); } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">
        ///     True for Ascending
        ///     False for Descending
        /// </param>
        /// <param name="nullsFirst">
        ///     null for Default
        ///     true for nulls first
        ///     false for nulls last
        /// </param>
        public record SqlSortType(bool value, bool? nullsFirst = null)
        {

            public string GetSortType()
            {
                return value == true ? "ASC" : "DESC";
            }

            public int GetNullsIndex()
            {
                int index = 0;
                if (value == true && nullsFirst == false ||
                    value == false && nullsFirst == true) index = 1;
                return index;

            }
        }


    }
}
