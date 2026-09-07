using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.Extensions.Localization;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.Share.Model.EditModel
{
    public class UserEditModel : EditBaseModel
    {
        public Property<UserEditModel> Property { get; set; } = new Property<UserEditModel>();

        public string Id { get; set; }
        [Display(Name = "Tên đăng nhập")]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(8, MinimumLength = 3, ErrorMessage = "Mật khẩu phải từ 3 đến 8 ký tự")]
        [RegularExpression(
        @"^(?=.*[@_]).*$",
        ErrorMessage = "Mật khẩu phải có ít nhất 1 ký tự đặc biệt (@ hoặc _)"
        )]
        public string PassWord { get; set; }
        [Display(Name = "Email cá nhân")]
        [Required]
        public string? Email { get; set; }
        public int? FailedLoginCount { get; set; }
        [Display(Name = "Họ và tên")]
        [Required]
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string Role { get; set; }
        public bool IsRegister { get; set; } = false;
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }
        public string Status { get; set; }
        public string LockReason { get; set; }


        public UserEditModel()
        {
            InputFields.Add<UserEditModel>(c => c.Email);
            InputFields.Add<UserEditModel>(c => c.Name);
        }

        public override Dictionary<string, List<string>> Validate(string nameProperty)
        {
            var Errors = new Dictionary<string, List<string>>();
            return Errors;
        }


    }
}
