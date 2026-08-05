using System.Text;

namespace NDS.Shared.Infrastructure.DataQuery.SqlQueryBuilders
{
    internal class SqlQueryJoinBuilder
    {
        private readonly List<SqlJoinData> _joinsData;

        public SqlQueryJoinBuilder()
        {
            _joinsData = new List<SqlJoinData>();
        }

        public void AddInnerJoin(string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            addJoin(SqlJoinType.Inner, mainTable, mainJoinField, joinTable, joinTableAlias, joinField);
        }

        public void AddLeftOuterJoin(string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            addJoin(SqlJoinType.LeftOuter, mainTable, mainJoinField, joinTable, joinTableAlias, joinField);
        }

        public void AddRightOuterJoin(string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            addJoin(SqlJoinType.RightOuter, mainTable, mainJoinField, joinTable, joinTableAlias, joinField);
        }

        public string GetJoinClauses()
        {
            if (_joinsData.Any())
            {
                StringBuilder builder = new StringBuilder();
                foreach (var join in _joinsData)
                {
                    builder.Append($"\n\t\t{join}");
                }
                builder.Remove(0, 1);
                return builder.ToString();
            }
            return string.Empty;
        }

        private void addJoin(SqlJoinType joinType, string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            if (joinTableAlias == null) { joinTableAlias = joinTable; }
            if (joinField == null) { joinField = mainJoinField; }
            _joinsData.Add(new SqlJoinData
            {
                MainTableAlias = mainTable,
                MainField = mainJoinField,
                JoinTable = joinTable,
                JoinTableAlias = joinTableAlias,
                JoinField = joinField,
                JoinType = joinType
            });
        }
    }
    internal class SqlQueryJoinsBuilder
    {
        private readonly List<SqlJoinMultiColumnsData> _joinsData;

        public SqlQueryJoinsBuilder()
        {
            _joinsData = new List<SqlJoinMultiColumnsData>();
        }

        public void AddInnerJoin(string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            addJoin(SqlJoinType.Inner, mainTable, mainJoinField, joinTable, joinTableAlias, joinField);
        }
        public void AddInnerJoin(string mainTable, string joinTable, string joinTableAlias, List<string[]> joinFields)
        {
            addJoin(SqlJoinType.Inner, mainTable, joinTable, joinTableAlias, joinFields);
        }

        public void AddLeftOuterJoin(string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            addJoin(SqlJoinType.LeftOuter, mainTable, mainJoinField, joinTable, joinTableAlias, joinField);
        }

        public void AddLeftOuterJoin(string mainTable, string joinTable, string joinTableAlias, List<string[]> joinFields)
        {
            addJoin(SqlJoinType.LeftOuter, mainTable, joinTable, joinTableAlias, joinFields);
        }

        public void AddRightOuterJoin(string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            addJoin(SqlJoinType.RightOuter, mainTable, mainJoinField, joinTable, joinTableAlias, joinField);
        }

        public void AddRightOuterJoin(string mainTable, string joinTable, string joinTableAlias, List<string[]> joinFields)
        {
            addJoin(SqlJoinType.RightOuter, mainTable, joinTable, joinTableAlias, joinFields);
        }

        public string GetJoinClauses()
        {
            if (_joinsData.Any())
            {
                StringBuilder builder = new StringBuilder();
                foreach (var join in _joinsData)
                {
                    builder.Append($"\n\t\t{join}");
                }
                builder.Remove(0, 1);
                return builder.ToString();
            }
            return string.Empty;
        }

        private void addJoin(SqlJoinType joinType, string mainTable, string mainJoinField, string joinTable, string? joinTableAlias = null, string? joinField = null)
        {
            if (joinTableAlias == null) { joinTableAlias = joinTable; }
            if (joinField == null) { joinField = mainJoinField; }
            _joinsData.Add(new SqlJoinMultiColumnsData
            {
                MainTableAlias = mainTable,
                JoinTable = joinTable,
                JoinTableAlias = joinTableAlias,
                JoinColumns = new List<string[]>() { new string[] { mainJoinField, joinField } },
                JoinType = joinType
            });
        }
        private void addJoin(SqlJoinType joinType, string mainTable, string joinTable, string joinTableAlias, List<string[]> joinFields)
        {
            if (joinTableAlias == null) { joinTableAlias = joinTable; }
            _joinsData.Add(new SqlJoinMultiColumnsData
            {
                MainTableAlias = mainTable,
                JoinTable = joinTable,
                JoinTableAlias = joinTableAlias,
                JoinColumns = joinFields,
                JoinType = joinType
            });
        }

    }


    internal class SqlJoinData
    {
        public string? MainTableAlias { get; set; }
        public string? MainField { get; set; }
        public string? JoinTable { get; set; }
        public string? JoinTableAlias { get; set; }
        public string? JoinField { get; set; }
        public SqlJoinType? JoinType { get; set; }

        public static implicit operator string(SqlJoinData joinData)
        {
            if (string.IsNullOrEmpty(joinData.MainTableAlias)) throw new Exception(nameof(joinData.MainTableAlias));
            if (string.IsNullOrEmpty(joinData.JoinTable)) throw new Exception(nameof(joinData.JoinTable));
            if (string.IsNullOrEmpty(joinData.MainField)) throw new Exception(nameof(joinData.MainField));
            if (string.IsNullOrEmpty(joinData.JoinField)) throw new Exception(nameof(joinData.JoinField));
            if (string.IsNullOrEmpty(joinData.JoinType)) throw new Exception(nameof(joinData.JoinType));

            var joinTableAlias = string.IsNullOrEmpty(joinData.JoinTableAlias) ? string.Empty : joinData.JoinTableAlias;

            return $"{joinData.JoinType} [{joinData.JoinTable}] AS [{joinTableAlias}] ON [{joinData.MainTableAlias}].[{joinData.MainField}] = [{joinTableAlias}].[{joinData.JoinField}] ";
        }

        public override string ToString()
        {
            return this;
        }
    }

    internal class SqlJoinMultiColumnsData
    {
        public string? MainTableAlias { get; set; }
        public string? JoinTable { get; set; }
        public string? JoinTableAlias { get; set; }

        public List<string[]> JoinColumns { get; set; } = new List<string[]>();
        public SqlJoinType? JoinType { get; set; }

        public static implicit operator string(SqlJoinMultiColumnsData joinData)
        {
            if (string.IsNullOrEmpty(joinData.MainTableAlias)) throw new Exception(nameof(joinData.MainTableAlias));
            if (string.IsNullOrEmpty(joinData.JoinTable)) throw new Exception(nameof(joinData.JoinTable));
            if (joinData.JoinColumns is null || !joinData.JoinColumns.Any()) throw new Exception(nameof(joinData.JoinColumns));
            if (string.IsNullOrEmpty(joinData.JoinType)) throw new Exception(nameof(joinData.JoinType));

            var joinTableAlias = string.IsNullOrEmpty(joinData.JoinTableAlias) ? string.Empty : joinData.JoinTableAlias;

            if (joinData.JoinColumns.Count == 1)

                return $"{joinData.JoinType} {joinData.JoinTable} AS {joinTableAlias} ON [{joinData.MainTableAlias}].[{joinData.JoinColumns[0][0]}] = [{joinTableAlias}].[{joinData.JoinColumns[0][1]}] ";

            StringBuilder builder = new StringBuilder($"{joinData.JoinType} [{joinData.JoinTable}] AS [{joinTableAlias}] ON");
            var isNotFirst = false;
            foreach (var column in joinData.JoinColumns)
            {
                if (isNotFirst)
                {
                    builder.Append("\n AND ");

                }

                builder.AppendLine($"[{joinData.MainTableAlias}].[{column[0]}] = [{joinTableAlias}].[{column[1]}] ");
                isNotFirst = true;
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            return this;
        }
    }
    internal class SqlJoinType
    {
        private readonly string _joinType;
        private SqlJoinType(string joinType)
        {
            _joinType = joinType;
        }
        public static SqlJoinType Inner { get { return new SqlJoinType(" INNER JOIN "); } }
        public static SqlJoinType LeftOuter { get { return new SqlJoinType(" LEFT OUTER JOIN "); } }
        public static SqlJoinType RightOuter { get { return new SqlJoinType(" RIGHT OUTER JOIN "); } }

        public static implicit operator string?(SqlJoinType? joinType)
        {
            return joinType?._joinType;
        }

        public override string ToString()
        {
            return _joinType;
        }

    }
}
