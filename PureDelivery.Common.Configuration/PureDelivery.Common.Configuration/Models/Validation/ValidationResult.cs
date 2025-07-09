using PureDelivery.Common.Configuration.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureDelivery.Common.Configuration.Models.Validation
{
    /// <summary>
    /// Результат валидации конфигурации
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Валидна ли конфигурация
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Список ошибок валидации
        /// </summary>
        public List<ValidationError> Errors { get; set; } = new();

        /// <summary>
        /// Создать успешный результат
        /// </summary>
        public static ValidationResult Success()
        {
            return new ValidationResult { IsValid = true };
        }

        /// <summary>
        /// Создать результат с ошибками
        /// </summary>
        public static ValidationResult WithErrors(params ValidationError[] errors)
        {
            return new ValidationResult
            {
                IsValid = false,
                Errors = errors.ToList()
            };
        }

        /// <summary>
        /// Создать результат с ошибками из строк
        /// </summary>
        public static ValidationResult WithErrors(params string[] errorMessages)
        {
            var errors = errorMessages.Select(msg => new ValidationError(msg)).ToArray();
            return WithErrors(errors);
        }
    }
}
