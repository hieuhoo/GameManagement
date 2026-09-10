using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Share.Model.EditModel
{
    public class GameRedeemCodeEditModel : EditBaseModel
    {
        public Property<GameRedeemCodeEditModel> Property { get; set; } = new Property<GameRedeemCodeEditModel>();

        public string Id { get; set; }

        [Display(Name = "Mã nạp")]
        public string Code { get; set; }

        [Display(Name = "Giá trị")]
        [Required]
        public int Value { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        public DateTime CreateDate { get; set; }

        [Display(Name = "Cách tạo")]
        public string GenerateType { get; set; } = GenerateRedeemType.ByHand.ToString();

        public string? CreateBy { get; set; }

        [Display(Name = "Ngày hết hạn")]
        [Required]
        public DateTime ExpiredDate { get; set; }

        [Display(Name = "Người sử dụng")]
        public string RedeemedBy { get; set; }

        [Display(Name = "Ngày sử dụng")]
        public DateTime? RedeemedDate { get; set; }
        public string? BatchId { get; set; }
        [Display(Name = "Số lượng")]
        public int? QuantityCode { get; set; }

        public GameRedeemCodeEditModel()
        {
            InputFields.Add<GameRedeemCodeEditModel>(c => c.Value);
            InputFields.Add<GameRedeemCodeEditModel>(c => c.Code);
            InputFields.Add<GameRedeemCodeEditModel>(c => c.QuantityCode);

        }

        public override Dictionary<string, List<string>> Validate(string nameProperty)
        {
            var Errors = new Dictionary<string, List<string>>();
            if (nameProperty == Property.Name(c => c.Value))
            {
                if (Value < 1000)
                {
                    Errors.AddExist(nameProperty, "Giá trị mã nạp phải >= 1000 ");
                }
            }
            if (nameProperty == Property.Name(c => c.Code))
            {
                if (GenerateType == GenerateRedeemType.ByHand.ToString() && Code.IsNullOrEmpty())
                {
                    Errors.AddExist(nameProperty, "Mã code bắt buộc nhập");
                }
            }
            if (nameProperty == Property.Name(c => c.QuantityCode))
            {
                if (GenerateType == GenerateRedeemType.Auto.ToString() && QuantityCode == null)
                {
                    Errors.AddExist(nameProperty, "Vui lòng nhập số lượng mã code để tự gen");
                }
            }
            return Errors;
        }
    }
}
