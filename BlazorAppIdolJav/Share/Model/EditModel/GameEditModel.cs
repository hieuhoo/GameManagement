using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Share.Model.EditModel
{
    public class GameEditModel : EditBaseModel
    {
        public Property<GameEditModel> Property { get; set; } = new Property<GameEditModel>();

        public string Id { get; set; }

        [Display(Name = "Tên game")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Quốc gia xuất bản")]
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
        public DateTime CreateDate { get; set; }

        [Display(Name = "Ngày ra mắt")]
        [Required]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Trạng thái")]
        [Required]
        public string Status { get; set; }

        [Display(Name = "Trạng thái bán hàng")]
        public string SoldStatus { get; set; }

        [Display(Name = "Giá tiền")]
        [Required]
        public int Price { get; set; }

        [Display(Name = "Đơn vị tiền")]
        [Required]
        public string Unit { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Nền tảng hỗ trợ")]
        [Required]
        public string SystemSupport { get; set; }

        [Display(Name = "Thuộc hãng game")]
        [Required]
        public string GameCompanyId { get; set; }

        [Display(Name = "Thể loại game")]
        [Required]
        public string GameTypeId { get; set; }

        public GameEditModel()
        {
            InputFields.Add<GameEditModel>(c => c.Price);

            DataSource[Property.NameProperty(c => c.Unit)] = Enum.GetValues(typeof(UnitMoneyEnum)).Cast<UnitMoneyEnum>()
                   .ToDictionary(c => c.ToString(), v => (ISelectItem)new SelectItem(v.ToString(), v.GetDescription()));
        }

        public override Dictionary<string, List<string>> Validate(string nameProperty)
        {
            var Errors = new Dictionary<string, List<string>>();
            if (nameProperty == Property.Name(c => c.Price))
            {
                if (Price <= 0)
                {
                    Errors.AddExist(nameProperty, "Giá tiền của game phải lớn hơn 0");
                }
            }
            return Errors;
        }

    }
}
