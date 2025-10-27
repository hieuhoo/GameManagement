using GameManagement.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Model.EditModel
{
    public class GameTypeEditModel : EditBaseModel
    {
        public Property<GameTypeEditModel> Property { get; set; } = new Property<GameTypeEditModel>();

        public string Id { get; set; }

        [Display(Name = "Tên thể loại")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Tên tiếng anh")]
        [Required]
        public string EnglishName { get; set; }

        [Display(Name = "Trạng thái")]
        [Required]
        public string Status { get; set; }

        public DateTime CreateDate { get; set; }

        [Display(Name = "Mô tả ngắn")]
        [Required]
        public string Description { get; set; }
    }
}
