using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BlazorAppIdolJav.SpecialComponent.ExtensionClass
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _conditionProperty;
        private readonly object _expectedValue;

        public RequiredIfAttribute(string conditionProperty, object expectedValue)
        {
            _conditionProperty = conditionProperty;
            _expectedValue = expectedValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(_conditionProperty);
            if (property == null)
                return new ValidationResult($"Không tìm thấy thuộc tính {_conditionProperty}");

            var conditionValue = property.GetValue(validationContext.ObjectInstance);

            if (conditionValue?.Equals(_expectedValue) == true && string.IsNullOrWhiteSpace(Convert.ToString(value)))
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} bắt buộc nhập.");
            }

            return ValidationResult.Success;
        }
    }
}
