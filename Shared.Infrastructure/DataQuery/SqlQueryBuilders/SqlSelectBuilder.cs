using Shared.Application.DataQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Shared.Application.DataQuery.JoinTable;

namespace NDS.Shared.Infrastructure.DataQuery.SqlQueryBuilders
{
    internal class SqlSelectBuilder
    {
        private readonly List<SqlSelectField>? _fields;
        public SqlSelectBuilder()
        {
            _fields = new List<SqlSelectField>();
        }


        public void AddFields(string fieldsData, string tableAlias)
        {
            if (string.IsNullOrEmpty(fieldsData)) 
                throw new ArgumentNullException(nameof(fieldsData));
            if(string.IsNullOrEmpty(tableAlias))
                throw new ArgumentNullException(nameof(tableAlias));

            var fieldsList = fieldsData.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            SetFields(fieldsList, tableAlias);


        }

        public void AddFields(IEnumerable<string> fieldsData, string tableAlias)
        {
            if(string.IsNullOrEmpty(tableAlias))
                throw new ArgumentNullException(nameof(tableAlias));
            SetFields(fieldsData, tableAlias);

        }

        public string GetSelectClause(string tableName, string? tableAlias = null, SelectOptions? selectOptions = null)
        {
            if (_fields is null)
                throw new ArgumentNullException(nameof(_fields));
            if (_fields.Any())
            {
                if (tableAlias is null) tableAlias = tableName;
                StringBuilder builder = new StringBuilder(" SELECT ");
                if (selectOptions != null)
                {
                    if (selectOptions.Distinct == true)
                    {
                        builder.Append(" DISTINCT ");
                    }

                    if (selectOptions.Top == true && selectOptions.TopValue > 0)
                    {
                        var percent = string.Empty;
                        var withTies = string.Empty;

                        if (selectOptions.Percent == true)
                        {
                            percent = "PERCENT";
                            if (selectOptions.TopValue > 100) selectOptions.TopValue = 100;
                        }

                        if (selectOptions.WithTies == true)
                        {
                            withTies = "WITH TIES";
                        }
                        builder.Append($" TOP ( {selectOptions.TopValue} ) {percent} {withTies} ");
                    }
                }
                builder.AppendLine();
                foreach (var fieldData in _fields)
                {
                    builder.AppendLine($"\t\t{fieldData.GetSelectName()} , ");
                }
                builder.Remove(builder.Length - 4, 4);
                builder.AppendLine();
                builder.Append($" FROM\t{tableName} AS {tableAlias}");
                return builder.ToString();
            }
            return string.Empty;
        }


        private void SetFields(IEnumerable<string> fieldsData, string tableAlias)
        {
            foreach (var fieldData in fieldsData)
            {

                _fields!.Add(new SqlSelectField($"{tableAlias}.{fieldData.Trim()}" ));
            }
        }

    }


    internal class SqlSelectField
    {
        public SqlSelectField(string fieldName, string fieldAlias, string tableNameWithSchemaOrtableAlias)
        {
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentNullException(fieldName);
            if (string.IsNullOrEmpty(fieldAlias)) throw new ArgumentNullException(fieldAlias);
            if (tableNameWithSchemaOrtableAlias == null) throw new ArgumentNullException(tableNameWithSchemaOrtableAlias);

            this.FieldName = StringToSQLElementCleaner.CleanOne(fieldName);
            this.AliasName = StringToSQLElementCleaner.CleanOne(fieldAlias);
            var table = StringToTableConverter.ToTableWithSchema(tableNameWithSchemaOrtableAlias);
            if (table != null && table.Length < 3)
            {
                if (table.Length == 1)
                {
                    this.TableAlias = table[0];
                }
                else
                {
                    this.SchemaName = table[0];
                    this.TableName = table[1];
                }
            }
            else
            {
                throw new ArgumentException($"Argument [{nameof(tableNameWithSchemaOrtableAlias)}] is not in valid format.");
            }
        }
        public SqlSelectField(string fieldData)
        {
            if(string.IsNullOrEmpty(fieldData)) throw new ArgumentNullException(fieldData);

            var data=StringToSQLElementCleaner.Clean(fieldData);
            var columnData = data.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if(columnData.Length > 2)
            {
                throw new ArgumentException($"Argument [{nameof(fieldData)}] is not in a valid format.");
            }
            if (columnData.Length == 2) {
                this.AliasName = StringToSQLElementCleaner.CleanOne(columnData[0]);
            }
            var columnNameData = columnData[0].Split(new[] { ' ', '[', ']', ',', ';', '.' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!columnNameData.Any() || columnNameData.Length > 3)
                throw new ArgumentException($"Argument with value [{fieldData}] can't converted to a valid columnn name.");

            if (columnNameData.Length == 1) {
                this.FieldName = columnData[0];

                
            }
            else if (columnNameData.Length == 2)
            {
                this.TableAlias= columnNameData[0];
                this.FieldName = columnNameData[1];
            }
            else
            {
                this.SchemaName = columnNameData[0];
                this.TableName = columnNameData[1];
                this.FieldName = columnNameData[2];
            }
        }
        public string? SchemaName { get; set; }
        public string? TableName { get; set; }
        public string? TableAlias { get; set; }
        public string? FieldName { get; set; }
        public string AliasName { get; set; } = string.Empty;


        public string GetSelectName()
        {
            return GetFullName();
        }



        private string GetFullName()
        {
            var tableName = string.Empty;
            if (!string.IsNullOrEmpty(this.TableAlias)){
                tableName= $"[{TableAlias.Trim()}].";
            }
            else
            {
                tableName = $"{(string.IsNullOrEmpty(SchemaName) ? string.Empty : $"[{SchemaName.Trim()}].")}{(string.IsNullOrEmpty(TableName) ? string.Empty : $"[{TableName}].")}";

            }
            var alias = string.IsNullOrEmpty(AliasName) ? string.Empty : $"[{AliasName}]";

            return $" {tableName}[{FieldName}] [{alias}] ";

            

        }

        public static implicit operator string(SqlSelectField field)
        {
            if(field is null)
                throw new ArgumentNullException(nameof(field));
            return field.GetFullName();
        }


        public static implicit operator SqlSelectField(string field)
        {
            return new SqlSelectField(field);
        }



    }

    internal class SqlSelectOptions
    {
        public bool Distinct { get; set; }
        public bool Top { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }
        public int TopValue { get; set; }
    }
}
