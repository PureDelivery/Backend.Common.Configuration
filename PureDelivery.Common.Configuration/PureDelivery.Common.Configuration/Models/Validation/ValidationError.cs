using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Models.Validation
{
    /// <summary>
    /// Ошибка валидации
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Название поля с ошибкой
        /// </summary>
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Значение поля
        /// </summary>
        public object? Value { get; set; }

        public ValidationError() { }

        public ValidationError(string message)
        {
            Message = message;
        }

        public ValidationError(string fieldName, string message, object? value = null)
        {
            FieldName = fieldName;
            Message = message;
            Value = value;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(FieldName)
                ? Message
                : $"{FieldName}: {Message}";
        }
    }
}
