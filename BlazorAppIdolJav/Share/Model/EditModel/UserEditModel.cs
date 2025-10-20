using BlazorAppIdolJav.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;

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
        public string? Email { get; set; }
        public int? PhoneNumber { get; set; }
        public int? QuantityLoginCount { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
