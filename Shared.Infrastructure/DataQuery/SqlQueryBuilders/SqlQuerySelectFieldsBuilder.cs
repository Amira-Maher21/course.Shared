using Shared.Application.DataQuery;
using System.Text;
using System.Text.RegularExpressions;

namespace NDS.Shared.Infrastructure.DataQuery.SqlQueryBuilders
{
    internal class SqlQuerySelectFieldsBuilder
    {
        private readonly List<SqlSelectNationalField>? _fields;

        public SqlQuerySelectFieldsBuilder()
        {
            _fields = new List<SqlSelectNationalField>();
        }


        public void AddFields(string? fieldsData, string? tableAlias = null, NationalFieldsOption nationalOption = NationalFieldsOption.SecondIfNullWithBrackets)
        {
            if (string.IsNullOrEmpty(fieldsData)) throw new Exception("Null Fields Data.");


            var fieldsList = fieldsData.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            SetFields(fieldsList, tableAlias, nationalOption);


        }

        public void AddFields(IEnumerable<string> fieldsData, string? tableAlias = null, NationalFieldsOption nationalOption = NationalFieldsOption.SecondIfNullWithBrackets)
        {
            SetFields(fieldsData, tableAlias, nationalOption);

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


        private void SetFields(IEnumerable<string> fieldsData, string? tableAlias = null, NationalFieldsOption nationalOption = NationalFieldsOption.SecondIfNullWithBrackets)
        {
            foreach (var fieldData in fieldsData)
            {

                _fields!.Add(new SqlSelectNationalField(fieldData.Trim(), tableAlias, nationalOption));
            }
        }
    }

    

    internal class SqlSelectNationalField
    {
        public SqlSelectNationalField(string? fieldData, string? tableAlias = null, NationalFieldsOption nationalOption = NationalFieldsOption.SecondIfNullWithBrackets)
        {
            if (string.IsNullOrEmpty(fieldData)) throw new Exception("Null Field Data.");

            fieldData = fieldData.Trim();
            fieldData = Regex.Replace(fieldData, @"\s+", " ");
            fieldData = Regex.Replace(fieldData, @"\s*\.\s*", ".");
            fieldData = Regex.Replace(fieldData, @"[\s*\[\s*]+", "[");
            fieldData = Regex.Replace(fieldData, @"[\s*\]\s*]+", "]");



            var field = fieldData.Split(new[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (field.Length == 1 || field.Length == 2)
            {
                setFieldName(field[0]);
                if (field.Length == 2)
                {
                    AliasName = field[1];
                }
                NationalOption = nationalOption;
                TableAlias = tableAlias;
            }
            else
            {
                throw new Exception("Invalid Field Data.");
            }

        }
        public string? SchemaName { get; set; }
        public string? TableName { get; set; }
        public string? TableAlias { get; set; }
        public string? FieldName { get; set; }
        public string? NationalFieldName { get; set; }
        public NationalFieldsOption NationalOption { get; set; }
        public string AliasName { get; set; } = string.Empty;
        public bool? IsNationalField { get; set; } = null;


        public string GetSelectName()
        {
            return GetFullName();
        }



        private string GetFullName()
        {

            var tableName = !string.IsNullOrEmpty(TableAlias) ? TableAlias : TableName;

            var schema = string.IsNullOrEmpty(SchemaName) ? string.Empty : $"[{SchemaName.Trim()}].";
            var table = string.IsNullOrEmpty(tableName) ? string.Empty : $"[{tableName}].";
            var alias = string.IsNullOrEmpty(AliasName) ? string.Empty : $"[{AliasName}]";
            var localName = $"{(string.IsNullOrEmpty(table) ? string.Empty : schema)}{table}[{FieldName}] {alias}";

            if (IsNationalField != null)
            {
                string selectFunction;
                if (string.IsNullOrEmpty(NationalFieldName)) NationalFieldName = $"{FieldName}_En";
                var nationalName = $"{schema}{table}[{NationalFieldName}]";
                string firstField, secondField;
                if (IsNationalField == true)
                {
                    firstField = nationalName;
                    secondField = localName;
                }
                else
                {

                    firstField = localName;
                    secondField = nationalName;

                }
                switch (NationalOption)
                {
                    case NationalFieldsOption.OneLanguageOnly: selectFunction = $" ISNULL( {firstField} , N'-' ) "; break;
                    case NationalFieldsOption.TowLanguagesWithBrackets: selectFunction = $" ISNULL( {firstField} , N'-' ) + ' [' + ISNULL( {secondField} , N'-' ) + '] '"; break;
                    case NationalFieldsOption.TowLanguagesWithSeparator: selectFunction = $" ISNULL( {firstField} , N'-' ) + ' / ' + ISNULL( {secondField} , N'-' )"; break;
                    case NationalFieldsOption.TowFields: selectFunction = $" {nationalName} [{NationalFieldName}] , {localName}"; break;
                    case NationalFieldsOption.SecondIfNullWithBrackets: selectFunction = $" ISNULL( {firstField},N'- [' +{secondField}+ ']' ) "; break;
                    case NationalFieldsOption.SecondIfNullWithoutBrackets: selectFunction = $" ISNULL( {firstField},{secondField} ) "; break;
                    default: throw new Exception("Unknown National Fields Option!...");
                }


                return selectFunction;
            }
            return localName;

        }

        private void setFieldName(string field)
        {
            if (!string.IsNullOrEmpty(field))
            {
                if (field.Contains("."))
                {
                    var fieldName = field.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < fieldName.Length; i++)
                    {
                        fieldName[i] = fieldName[i].Trim();
                    }

                    if (fieldName.Length == 1)
                    {
                        FieldName = fieldName[0];
                    }
                    else if (fieldName.Length == 2)
                    {
                        TableName = fieldName[0];
                        FieldName = fieldName[1];
                    }
                    else if (fieldName.Length == 3)
                    {
                        SchemaName = fieldName[0];
                        TableName = fieldName[1];
                        FieldName = fieldName[2];
                    }
                }
                else
                {
                    FieldName = field;
                }

            }
        }


        public static implicit operator string(SqlSelectNationalField field)
        {
            var fieldAlias = string.IsNullOrEmpty(field.AliasName) ? field.FieldName : $"{field.AliasName.Trim()}";

            return $" {field.GetFullName} [{fieldAlias}] ";
        }


        public static implicit operator SqlSelectNationalField(string field)
        {
            return new SqlSelectNationalField(field);
        }



    }


    internal class SelectOptions
    {
        public bool Distinct { get; set; }
        public bool Top { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }
        public int TopValue { get; set; }
    }


}
