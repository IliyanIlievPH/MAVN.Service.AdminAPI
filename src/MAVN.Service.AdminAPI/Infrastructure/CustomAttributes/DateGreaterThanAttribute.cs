using System;
using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.AdminAPI.Infrastructure.CustomAttributes
{
    /// <inheritdoc />
    /// <summary>
    /// Custom DataValidation attribute that checks if FromDate is before ToEnd
    /// </summary>
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public DateGreaterThanAttribute(string startDate)
        {
            _startDatePropertyName = startDate;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (!(value is DateTime) || !(propertyValue is DateTime))
            {
                return new ValidationResult("Incorrect object type");
            }

            if ((DateTime)value > (DateTime)propertyValue)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(validationContext.DisplayName + " must be after " + _startDatePropertyName+ ".");
        }
    }
}
