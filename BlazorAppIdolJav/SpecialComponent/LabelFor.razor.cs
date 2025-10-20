using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace BlazorAppIdolJav.SpecialComponent
{
    public partial class LabelFor : ComponentBase
    {
        protected string Name { get; set; }
        protected string Display { get; set; }
        protected bool Required { get; set; }
        [CascadingParameter] IStringLocalizer Localizer { get; set; }
        [Parameter] public FieldIdentifier FieldIdentifier { get; set; }

        [Parameter] public Expression<Func<string>> For { get; set; }

        [Parameter] public string Title { get; set; }
        [Parameter] public bool HasRequired { get; set; } = false;
        [Parameter] public bool IsEllipsis { get; set; } = false;
        [Parameter] public bool IsEnableTooltip { get; set; } = false;
        [Parameter] public Placement TooltipPlacement { get; set; } = Placement.Top;

        protected override void OnParametersSet()
        {
            if (For == null)
            {
                if (FieldIdentifier.Model == null)
                    throw new InvalidOperationException("For or FieldIdentity is required.");
            }
            else
            {
                FieldIdentifier = FieldIdentifier.Create(For);
            }

            Name = FieldIdentifier.FieldName;
            var property = FieldIdentifier.Model.GetType().GetProperty(Name);
            if (property != null)
            {
                var displayAttribute = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayAttribute), false)?.FirstOrDefault();
                if (displayAttribute != null)
                {
                    Display = displayAttribute.Name;
                    if (Localizer != null)
                    {
                        Display = Localizer[displayAttribute.Name];
                    }
                }
                else
                {
                    Display = Name;
                }
            }
            else
            {
                throw new InvalidOperationException("For is used for property.");
            }

            if (property != null)
            {
                Required = property.GetCustomAttributes(typeof(RequiredAttribute), false).Any();
            }
            else
            {
                throw new InvalidOperationException("For is used for property.");
            }
        }
    }
}

