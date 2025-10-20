using BlazorAppIdolJav.CoreConfig.Extensions;
using BlazorAppIdolJav.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;

namespace BlazorAppIdolJav.Share.Model.EditModel
{
    public class ActressEditModel : EditBaseModel
    {
        public Property<ActressEditModel> Property { get; set; } = new Property<ActressEditModel>();

        public string Id { get; set; }

        [Display(Name = "Họ và tên")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Quốc tịch")]
        [Required]
        public string Country { get; set; }
        [Display(Name = "Ảnh đại diện")]
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public bool IsInUnsafeUpload { get; set; }

        public string ImagePathView
        {
            get
            {
                if (ImagePath.IsNotNullOrEmpty())
                {
                    if (IsInUnsafeUpload)
                    {
                        return ImagePath;
                    }
                    else
                    {
                        return Path.Combine(GlobalVariant.UploadFolderResource, ImagePath);
                    }
                }
                else
                {
                    return "";
                }
            }
            set { }
        }
        [Display(Name = "Tuổi")]
        [Required]
        public int Age { get; set; }
        [Display(Name = "Số đo vòng 1")]
        [Required]

        public int ChestSize { get; set; }
        [Display(Name = "Số đo vòng 3")]
        [Required]

        public int ButtSize { get; set; }
        [Display(Name = "Số đo vòng 2")]
        [Required]

        public int WaistSize { get; set; }
        public DateTime CreateDate { get; set; }

        [Display(Name = "Ngày ra mắt")]
        [Required]
        public DateTime? DebutDate { get; set; }
        public string CompanyId { get; set; }
        [Display(Name = "Giới tính")]
        [Required]
        public string Gender { get; set; }
        [Display(Name = "Biệt danh")]
        public string NickName { get; set; }
        
    }
}
