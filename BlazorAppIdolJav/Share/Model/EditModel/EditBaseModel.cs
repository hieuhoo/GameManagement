using AutoMapper.Internal;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameManagement.Share.Model.EditModel
{
    public abstract class EditBaseModel
    {
        public Dictionary<string, Dictionary<string, ISelectItem>> DataSource { get; set; } = new Dictionary<string, Dictionary<string, ISelectItem>>();
        public List<PropertyInfo> InputFields { get; set; } = new List<PropertyInfo>();
        public List<string> FieldsChanged { get; set; } = new List<string>();
        public HashSet<string> DisableInputFields { get; set; } = new HashSet<string>();
        public HashSet<string> RequiredInputFields { get; set; } = new HashSet<string>();
        public event Action OnChange;
        public IStringLocalizer Localizer { get; set; }
        public IStringLocalizer EnumsLocalizer { get; set; }

        public bool RequiredRefresh { get; set; }
        public bool ReadOnly { get; set; }
        public bool DataChanged { get; set; }
        public bool IsNew { get; set; }
        public bool IsInit { get; set; }

        int hashCode;
        public virtual Task InitFormAsync()
        {
            return Task.FromResult(default(object));
        }

        public void InitForm()
        {
            InitFormAsync();
        }
        public virtual void LoadViewCombobox()
        {

        }
        public virtual Dictionary<string, List<string>> Validate(string nameProperty)
        {
            return null;
        }

        public virtual Dictionary<string, List<string>> ValidateAll(bool breakIfError = false)
        {
            return ValidateInput(InputFields, breakIfError);
        }

        public virtual Dictionary<string, List<string>> ValidateInput(List<PropertyInfo> inputs, bool breakIfError = false)
        {
            Dictionary<string, List<string>> validateMessageStore = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> validateMessage;
            foreach (var field in (inputs ?? InputFields))
            {
                validateMessage = Validate(field.Name);
                if (validateMessage != null && validateMessage.Any())
                {
                    if (breakIfError)
                    {
                        var message = validateMessage.First();
                        validateMessageStore.Add(message.Key, message.Value);
                        return validateMessageStore;
                    }
                    foreach (var messageStore in validateMessage)
                    {
                        if (validateMessageStore.ContainsKey(messageStore.Key))
                        {
                            continue;
                        }
                        validateMessageStore.Add(messageStore.Key, messageStore.Value);
                    }
                }
            }
            return validateMessageStore;
        }

        public virtual string GetCustomClassForField(string nameProperty)
        {
            return string.Empty;
        }

        public override int GetHashCode()
        {
            if (hashCode == 0)
            {
                hashCode = base.GetHashCode();
            }
            return hashCode;
        }

        public virtual void Change(string nameProperty, string value)
        {
            try
            {
                if (DisableInputFields.Contains(nameProperty))
                {
                    return;
                }
                var currentValue = this.GetValue(nameProperty);
                if (currentValue?.ToString() == value)
                {
                    return;
                }
                this.FieldsChanged.Clear();
                this.SetValue(nameProperty, value);
                FieldsChanged.Add(nameProperty);
                ChangeOtherField(nameProperty, value);
                FormChanged();
            }
            catch (Exception)
            {
            }
        }

        public virtual void Change(string nameProperty, object value)
        {
            try
            {
                if (DisableInputFields.Contains(nameProperty))
                {
                    return;
                }
                var currentValue = this.GetValue(nameProperty);
                //if (Object.Equals(currentValue, value))
                //{
                //    return;
                //}
                this.FieldsChanged.Clear();
                this.SetValue(nameProperty, value);
                FieldsChanged.Add(nameProperty);
                ChangeOtherField(nameProperty, value);
                FormChanged();
            }
            catch (Exception)
            {
            }
        }

        public void FormChanged()
        {
            DataChanged = true;
            OnChange?.Invoke();
        }

        protected virtual void ChangeOtherField<T>(string nameProperty, T value)
        {

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = ValidateAll();
            foreach (var error in errors)
            {
                foreach (var message in error.Value)
                {
                    yield return new ValidationResult(message, new[] { error.Key });
                }
            }
        }

        public List<SelectItem> GetDataSource<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null
                || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", expression));
            if (DataSource.TryGetValue(propertyInfo.Name, out var value))
            {
                return value.Values.Select(c => new SelectItem(c.GetKey(), c.GetDisplay())).ToList();
            }
            return new List<SelectItem>();
        }


        public virtual List<Dictionary<string, object>> ToFlatObject(string data, List<string> usedKeys, string tableAlias)
        {
            var convertData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data);
            return convertData;
        }

        public virtual List<string> GetMultipleDBField()
        {
            return new List<string>();
        }

        public string FormatColumnField(string tableAlias, string field)
        {
            if (field.IsNullOrEmpty())
            {
                return field;
            }
            return tableAlias + "." + field;
        }

        public string CustomError(string error)
        {
            return Localizer != null ? Localizer[error] : error;
        }
    }
}
