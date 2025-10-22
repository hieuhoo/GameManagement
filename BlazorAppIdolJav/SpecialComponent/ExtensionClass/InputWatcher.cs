using GameManagement.CoreConfig.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Reflection;

namespace GameManagement.SpecialComponent.ExtensionClass
{
    public class InputWatcher : ComponentBase
    {
        [CascadingParameter] EditContext CurrentEditContext { get; set; }

        [Parameter]
        public EventCallback<string> FieldChanged { get; set; }
        private ValidationMessageStore _messageStore;

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(InputWatcher)} requires a cascading " +
                    $"parameter of type {nameof(CurrentEditContext)}. For example, you can use {nameof(InputWatcher)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            _messageStore = new ValidationMessageStore(CurrentEditContext);
            CurrentEditContext.OnValidationRequested += (s, e) => _messageStore.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) => _messageStore.Clear(e.FieldIdentifier);
        }

        public bool Validate()
        => CurrentEditContext?.Validate() ?? false;

        public void NotifyFieldChanged(string propertyName)
        {
            CurrentEditContext?.NotifyFieldChanged(CurrentEditContext.Field(propertyName));
        }

        public void NotifyFieldsChanged(List<string> propertyNames)
        {
            propertyNames.ForEach(c => CurrentEditContext?.NotifyFieldChanged(CurrentEditContext.Field(c)));
        }

        public void NotifyFieldChanged(string propertyName, Dictionary<string, List<string>> errors)
        {
            NotifyFieldChanged(propertyName);
            if (errors != null)
            {
                foreach (var err in errors)
                {
                    NotifyFieldChanged(err.Key);
                    _messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
                }
            }
            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void NotifyFieldChanged(string propertyName, string message)
        {
            NotifyFieldChanged(propertyName);
            if (message.IsNotNullOrEmpty())
            {
                _messageStore.Add(CurrentEditContext.Field(propertyName), message);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearMessage()
        {
            _messageStore?.Clear();
            CurrentEditContext.MarkAsUnmodified();
        }

        public Dictionary<string, List<string>> GetValidationMessages(List<PropertyInfo> propertyInfos)
        {
            return propertyInfos
                .Select(c => new
                {
                    Key = c.Name,
                    Value = CurrentEditContext.GetValidationMessages(CurrentEditContext.Field(c.Name)).ToList()
                }).Where(c => c.Value.Any()).ToDictionary(c => c.Key, v => v.Value);
        }
    }
}
