using GameManagement.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Model.EditModel
{
    public class GameCompanyEditModel : EditBaseModel
    {
        public Property<GameCompanyEditModel> Property { get; set; } = new Property<GameCompanyEditModel>();

        public string Id { get; set; }

        [Display(Name = "Tên hãng game")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Quốc gia")]
        [Required]
        public string Country { get; set; }

        [Display(Name = "Trạng thái")]
        [Required]
        public string Status { get; set; }

        public DateTime CreateDate { get; set; }

        [Display(Name = "Mô tả ngắn")]
        [Required]
        public string Description { get; set; }

    }
}
