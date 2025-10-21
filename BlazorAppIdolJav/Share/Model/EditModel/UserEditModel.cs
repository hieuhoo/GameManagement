using BlazorAppIdolJav.CoreConfig;
using BlazorAppIdolJav.CoreConfig.Extensions;
using BlazorAppIdolJav.SpecialComponent.ExtensionClass;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using static BlazorAppIdolJav.Share.Extension.MessageEnumExtension;

namespace BlazorAppIdolJav.Share.Model.EditModel
{
    public class UserEditModel : EditBaseModel
    {
        public Property<UserEditModel> Property { get; set; } = new Property<UserEditModel>();

        public string Id { get; set; }
        [Display(Name = "Tên đăng nhập")]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required]
        public string PassWord { get; set; }
        [Display(Name = "Email cá nhân")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public int? PhoneNumber { get; set; }
        public int? QuantityLoginCount { get; set; }
        [Display(Name = "Họ và tên")]
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Role { get; set; }
        public bool IsRegister { get; set; } = false;

        public UserEditModel()
        {
            InputFields.Add<UserEditModel>(c => c.Email);
            InputFields.Add<UserEditModel>(c => c.PhoneNumber);
            InputFields.Add<UserEditModel>(c => c.Name);
        }

        public override Dictionary<string, List<string>> Validate(string nameProperty)
        {
            var Errors = new Dictionary<string, List<string>>();
            if (nameProperty == Property.Name(c => c.Email))
            {
                if (IsRegister)
                {
                    if (Email.IsNullOrEmpty())
                    {
                        Errors.AddExist(nameProperty, TypeAlert.Required.GetDescription());
                    }
                }
            }
            if (nameProperty == Property.Name(c => c.Name))
            {
                if (IsRegister)
                {
                    if (Name.IsNullOrEmpty())
                    {
                        Errors.AddExist(nameProperty, TypeAlert.Required.GetDescription());
                    }
                }
            }

            if (nameProperty == Property.Name(c => c.PhoneNumber))
            {
                if (IsRegister)
                {
                    if (!PhoneNumber.HasValue)
                    {
                        Errors.AddExist(nameProperty, TypeAlert.Required.GetDescription());
                    }
                }
            }
            return Errors;
        }


    }
}
