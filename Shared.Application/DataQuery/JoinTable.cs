using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shared.Application.DataQuery
{
    public class JoinTable
    {
        public JoinTable(string tableName, List<JoinSelectColumn> selectColumns, List<JoinColumn> joinColumns)
        {

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException($"Argument [{nameof(tableName)}] can't bet null or empty.");
            InitializeTableData(tableName);

            SelectColumns = selectColumns;
            JoinColumns = joinColumns;
        }

        public JoinTable(string tableName, string selectColumns, string joinColumns)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException($"Argument [{nameof(tableName)}] can't bet null or empty.");

            if (string.IsNullOrEmpty(selectColumns))

                throw new ArgumentException($"Argument [{nameof(selectColumns)}] can't be null or empty.");
            if (string.IsNullOrEmpty(selectColumns))
                throw new ArgumentException($"Argument [{nameof(joinColumns)}] can't be null or empty.");

            InitializeTableData(tableName);
            this.SelectColumns = StringToSelectColumnConverter.ToManySelectColumns(selectColumns);
            this.JoinColumns = StringToJoinColumnConverter.ToManyJoinColumns(joinColumns);

        }

        public string TableName { get; private set; }

        public string? SchemaName { get; private set; }

        public string TableNameWithSchema
        {
            get
            {
                return $"{(string.IsNullOrEmpty(SchemaName) ? string.Empty : $"[{SchemaName}].")}[{TableName}]";
            }
        }

        public IReadOnlyList<JoinSelectColumn> SelectColumns { get; private set; }
        public IReadOnlyList<JoinColumn> JoinColumns { get; private set; }

        [MemberNotNull(nameof(TableName))]
        private void InitializeTableData(string tableName) {
            var data = StringToSQLElementCleaner.Clean(tableName);

            var table=StringToTableConverter.ToTableWithSchema(data);
            if(!table.Any() || table.Length > 2)
            {
                throw new ArgumentException(nameof(tableName));

            }

            if(table.Length == 1)
                this.TableName= table[0];
            else
            {
                this.SchemaName= table[0];
                this.TableName= table[1];
            }
        }
    }

    public class JoinSelectColumn
    {
        public JoinSelectColumn(string columnName, string columnAliasName)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException($"Argument [{nameof(columnName)}] can't be null or empty.");

            if (string.IsNullOrEmpty(columnAliasName))
                throw new ArgumentNullException($"Argument [{nameof(columnAliasName)}] can't be null or empty.");


            this.ColumnName = StringToSQLElementCleaner.CleanOne(columnName);
            this.ColumnAliasName = StringToSQLElementCleaner.CleanOne(columnAliasName);
        }

        public JoinSelectColumn(string columnData)
        {
            if (string.IsNullOrEmpty(columnData))
                throw new ArgumentNullException($"Argument [{nameof(columnData)}] can't be null or empty.");


            var data = StringToSelectColumnConverter.ToOneColumn(columnData);

            if (!data.Any() || data.Length > 2)
                throw new ArgumentException($"Invalid argument [{nameof(columnData)}].");

            if (data.Length == 1)
            {
                ColumnName = data[0];
                ColumnAliasName = data[0];
            }
            else
            {
                ColumnName = data[0];
                ColumnAliasName = data[1];
            }
        }
        public string ColumnName { get; private set; }
        public string ColumnAliasName { get; private set; }

    }

    public class JoinColumn
    {
        public JoinColumn(string joinColumn)
        {

            if (string.IsNullOrEmpty(joinColumn))
                throw new ArgumentNullException($"Argument [{nameof(joinColumn)}] can't be null or empty.");

            var data = StringToSQLElementCleaner.Clean(joinColumn);
            var joinData = StringToJoinColumnConverter.ToOneJoinColumn(data);

            this.MainTableColumnName = joinData[0];
            this.JoinTableColumnName = joinData[1];

        }
        public JoinColumn(string mainTableColumn, string joinTableColumn)
        {
            if (string.IsNullOrEmpty(mainTableColumn))
                throw new ArgumentNullException($"Argument [{nameof(mainTableColumn)}] can't be null or empty.");

            if (string.IsNullOrEmpty(joinTableColumn))
                throw new ArgumentNullException($"Argument [{nameof(joinTableColumn)}] can't be null or empty.");


            this.MainTableColumnName = StringToSQLElementCleaner.CleanOne(mainTableColumn);
            this.JoinTableColumnName = StringToSQLElementCleaner.CleanOne(joinTableColumn);

        }
        public string JoinTableColumnName { get; private set; }
        public string MainTableColumnName { get; private set; }
    }

    internal static class StringToSelectColumnConverter
    {
        public static List<JoinSelectColumn> ToManySelectColumns(string columnsData)
        {
            var data = StringToSQLElementCleaner.Clean(columnsData);


            var fieldsData = data.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


            if (!fieldsData.Any())
                throw new ArgumentException($"Argument [{nameof(columnsData)}] with value [{columnsData}] can't converted to a valid select columns.");
            var columns = new List<JoinSelectColumn>();
            foreach (var column in fieldsData)
            {
                columns.Add(new JoinSelectColumn(column));
            }

            return columns;
        }

        public static string[] ToOneColumn(string columnData)
        {
            var data = StringToSQLElementCleaner.Clean(columnData);

            var fieldData = data.Split(new[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


            if (!fieldData.Any() || fieldData.Length > 2)
                throw new ArgumentException($"Argument {nameof(columnData)} not in a valid format.");

            if (fieldData.Length == 1)
                return new[] { fieldData[0], fieldData[0] };

            return fieldData;
        }

    }

    internal static class StringToJoinColumnConverter
    {
        public static List<JoinColumn> ToManyJoinColumns(string joinData)
        {
            var data = StringToSQLElementCleaner.Clean(joinData);

            var joinColumns = data.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!joinColumns.Any())
                throw new ArgumentException($"Argument [{nameof(joinData)}] with value [{joinData}] can't converted to a valid join.");
            var joins = new List<JoinColumn>();
            foreach (var join in joinColumns)
            {
                joins.Add(new JoinColumn(join));
            }

            return joins;

        }

        public static string[] ToOneJoinColumn(string joinData)
        {
            var data = StringToSQLElementCleaner.Clean(joinData);

            var join = data.Split(new[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!join.Any() || join.Length > 2)
                throw new ArgumentException($"Argument {nameof(joinData)} with value [{joinData}] can't converted to a valid join.");

            if (join.Length == 1)
                return new[] { join[0], join[0] };

            return join;

        }
    }
    public static class StringToTableConverter
    {
        public static string[] ToTableWithSchema(string tableData)
        {
            var data = StringToSQLElementCleaner.Clean(tableData);

            var table = data.Split(new[] { ' ', '[', ']', ',', ';', '.' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!table.Any() || table.Length > 2)
                throw new ArgumentException($"Argument with value [{tableData}] can't converted to a valid table name.");

            return table;

        }

    }
    public static class StringToSQLElementCleaner
    {
        public static string Clean(string stringData)
        {
            var data = stringData
                .Replace('\n', ' ')
                .Replace('\t', ' ')
                .Trim();
            data = Regex.Replace(data, @"\s+", " ");
            data = Regex.Replace(data, @"[\s*\[\s*]+", "[");
            data = Regex.Replace(data, @"[\s*\]\s*]+", "]");
            data = Regex.Replace(data, @"[\s*\.\s*]+", ".");

            return data;
        }

        public static string CleanOne(string stringData)
        {
            var data = Clean(stringData);

            var name = data.Split(new[] { ' ', '[', ']', ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!name.Any() || name.Length != 1)
                throw new ArgumentException($"Argument with value [{stringData}] can't converted to single SQl element name.");

            return name[0];
        }
    }
}
