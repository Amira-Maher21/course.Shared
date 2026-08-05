using  Shared.Kernel.BaseReturnTypes;
using  Shared.Kernel.Exceptions;
using System.Text.RegularExpressions;

namespace  Shared.Infrastructure.Exceptions
{
    public class ExceptionManager : IExceptionManager
    {
        private readonly string _language;
        public ExceptionManager(string language = "English")
        {
            _language = language;
        }

        public ReturnBaseError GetErrorFromException(Exception exception)
        {
            return new ReturnBaseError
            {
                //Exception = exception,
                SystemErrorMessage = exception.Message,
                Source = exception.Source,
                ErrorMessage = GetErrorFromMessageAsync(exception.Message)
            };
        }

        public string GetErrorFromMessageAsync(string errorMessage)
        {
            try
            {
                return GetUserFriendlyMessage(errorMessage);
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
            if (string.IsNullOrEmpty(message))
                return _language == "Arabic" ? "حدث خطأ غير معروف" : "An unknown error occurred";

            if (message.Contains("Cannot insert the value NULL into column"))
                return HandleNoNullException(message);
            if (message.Contains("PRIMARY KEY constraint"))
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
            if (message.Contains("unique index") || message.Contains("duplicate key"))
                return HandleDuplicateKeyException(message);
            if (message.Contains("REFERENCE constraint"))
                return HandleReferenceConstrain(message);
            if (message.Contains("truncated"))
                return HandleLargeInput(message);

            return _language == "Arabic" ? "حدث خطأ غير معروف" : "An unknown error occurred";
        }
        private string HandleNoNullException(string message)
        {
            var columnMatch = Regex.Match(message, @"column '([^']*)'");
            var tableMatch = Regex.Match(message, @"table ['""]([^'""]*)['""]");

            string columnName = columnMatch.Success ? columnMatch.Groups[1].Value : "unknown";
            string tableName = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $"الحقل '{columnName}' في جدول '{tableName}' لا يمكن أن يكون فارغًا"
                : $"Field '{columnName}' in table '{tableName}' cannot be empty";
        }
        private string HandleDuplicateKeyException(string message)
        {
            var keyMatch = Regex.Match(message, @"with unique index '([^']*)'");
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
            return message;
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
            var tableMatch = Regex.Match(message, @"table ['""]([^'""]*)['""]");

            string columnName = columnMatch.Success ? columnMatch.Groups[1].Value : "unknown";
            string tableName = tableMatch.Success ? tableMatch.Groups[1].Value : "unknown";

            return _language == "Arabic"
                ? $" لايمكن مسح او تعديل حقل {columnName} حيث انه مستخدم فى {tableName}"
                : $"Field '{columnName}' in table '{tableName}' cannot be empty";
        }
        private string HandleLargeInput(string message)
        {
            return _language == "Arabic"
            ? $"غير مسموح بتخطي الحد الاقصي لعدد الأحرف"
            : $"Not allowed to exceed the max limit of input characters";
        }
    }
}
