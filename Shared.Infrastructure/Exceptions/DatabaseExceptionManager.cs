using Shared.Kernel.Exceptions;
using System.Text.RegularExpressions;

namespace NDS.Shared.Infrastructure.Exceptions
{
    public class DatabaseExceptionManager : IDatabaseExceptionManager
    {
        private readonly string _language;

        public DatabaseExceptionManager(string language = "English")
        {
            _language = language;
        }

        public async Task<string> GetErrorFromMessageAsync(string errorMessage)
        {
            try
            {
                return await Task.FromResult(GetUserFriendlyMessage(errorMessage));
            }
            catch (Exception ex)
            {
                return _language == "Arabic"
                    ? $"حدث خطأ غير متوقع: {ex.Message}"
                    : $"An unexpected error occurred: {ex.Message}";
            }
        }
        private string GetUserFriendlyMessage(string message)
        {
            if (message.Contains("Cannot insert the value NULL into column"))
                return HandleNoNullException(message);
            if (message.Contains("PRIMARY KEY constraint") || message.Contains("duplicate key"))
                return HandleDuplicateKeyException(message);
            if (message.Contains("FOREIGN KEY constraint"))
                return HandleForeignKeyConstraint(message);
            if (message.Contains("CHECK constraint"))
                return HandleCheckConstraint(message);
            if (message.Contains("Violation of PRIMARY KEY constraint"))
                return HandleViolationOfPRIMARYKEYConstraint();
            if (message.Contains("The transaction ended in the trigger. The batch has been aborted."))
                return HandleTriggerException(message);
            if (message.Contains("Divide by zero error"))
                return HandleDivideByZeroException(message);
            if (message.Contains("unique index"))
                return HandleDuplicateKeyException(message);
            if (message.Contains("REFERENCE constraint"))
                return HandleReferenceConstrain(message);
            return _language == "Arabic" ? "حدث خطأ غير معروف" : "An unknown error occurred";
        }
        private string HandleNoNullException(string message)
        {
            var columnMatch = Regex.Match(message, @"column '([^']*)'");
            var tableMatch = Regex.Match(message, @"table '([^']*)'");

            string columnName = columnMatch.Success ? columnMatch.Groups[1].Value : "unknown";
            string tableName = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $"الحقل '{columnName}' في جدول '{tableName}' لا يمكن أن يكون فارغًا"
                : $"Field '{columnName}' in table '{tableName}' cannot be empty";
        }
        private string HandleDuplicateKeyException(string message)
        {
            var keyMatch = Regex.Match(message, @"PRIMARY KEY constraint '([^']*)'");
            var objectMatch = Regex.Match(message, @"object '([^']*)'");
            var keyValueMatch = Regex.Match(message, @"The duplicate key value is \(([^)]*)\)");

            string keyName = keyMatch.Success ? keyMatch.Groups[1].Value : "unknown";
            string tableName = objectMatch.Success ? objectMatch.Groups[1].Value : "unknown";
            string keyValue = keyValueMatch.Success ? keyValueMatch.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $"لا يمكن إدراج قيمة مكررة في الجدول '{tableName}'. المفتاح '{keyName}' يتعارض مع القيمة ({keyValue})"
                : $"Cannot insert duplicate key in table '{tableName}'. Primary key '{keyName}' conflicts with value ({keyValue})";
        }
        private string HandleForeignKeyConstraint(string message)
        {
            var match = Regex.Match(message, @"FOREIGN KEY constraint\s+\""(.*?)\""");
            string constraintName = match.Success ? match.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $"لا يمكن إتمام العملية بسبب تعارض في المفتاح الأجنبي '{constraintName}'"
                : $"Operation cannot be completed due to a foreign key constraint '{constraintName}' conflict";
        }
        private string HandleCheckConstraint(string message)
        {
            var match = Regex.Match(message, @"CHECK constraint '([^']*)'");
            string constraintName = match.Success ? match.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $"البيانات المدخلة غير صحيحة بسبب تعارض مع القيد '{constraintName}'"
                : $"Invalid data due to a check constraint '{constraintName}' conflict";
        }
        private string HandleViolationOfPRIMARYKEYConstraint()
        {
            return _language == "Arabic"
                ? "البيانات الرئيسية موجودة مسبقا من فضلك قم بتعديل البيانات الرئيسية أو قم بإغلاق الشاشة وفتحها مرة أخرى"
                : $"Master data already exists, please modify the master data or close the screen and open again";
        }
        private string HandleTriggerException(string message)
        {
            var triggerMatch = Regex.Match(message, @"trigger '([^']*)'");
            var tableMatch = Regex.Match(message, @"table '([^']*)'");
            var errorCodeMatch = Regex.Match(message, @"error code (\d+)");

            string triggerName = triggerMatch.Success ? triggerMatch.Groups[1].Value : "unknown";
            string tableName = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown";
            string errorCode = errorCodeMatch.Success ? errorCodeMatch.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $"فشلت العملية بسبب قيود التريجر '{triggerName}' في الجدول '{tableName}'. رمز الخطأ: {errorCode}"
                : $"Operation failed due to trigger '{triggerName}' constraints on table '{tableName}'. Error Code: {errorCode}";
        }
        private string HandleDivideByZeroException(string message)
        {
            return _language == "Arabic"
                ? "لا يمكن إتمام هذه العملية حيث أن أرصدة القيم الخاصة بإحدى الأصناف تساوي صفر"
                : "Can not complete this process as the balances of the values for one of the items is equal to zero";
        }
        private string HandleReferenceConstrain(string message)
        {
            var columnMatch = Regex.Match(message, @"column '([^']*)'");
            var tableMatch = Regex.Match(message, @"table '([^']*)'");

            string columnName = columnMatch.Success ? columnMatch.Groups[1].Value : "unknown";
            string tableName = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $" لايمكن مسح او تعديل حقل {columnName} حيث انه مستخدم فى {tableName}"
                : $"Field '{columnName}' in table '{tableName}' cannot be empty";
        }
    }
}
