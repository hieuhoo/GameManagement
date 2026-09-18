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
		public string? Code { get; set; }

		[Display(Name = "Giá trị")]
		public int? Value { get; set; }

		[Display(Name = "Trạng thái")]
		public string Status { get; set; }

		public DateTime CreateDate { get; set; }

		[Display(Name = "Cách tạo")]
		public string GenerateType { get; set; }

		public string? CreateBy { get; set; }

		[Display(Name = "Ngày hết hạn")]
		[Required]
		public DateTime ExpiredDate { get; set; }

		[Display(Name = "Người sử dụng")]
		public string RedeemedBy { get; set; } // sẽ là Id

		[Display(Name = "Ngày sử dụng")]
		public DateTime? RedeemedDate { get; set; }
		public string? BatchId { get; set; }
		[Display(Name = "Số lượng")]
		public int? QuantityCode { get; set; }
		[Display(Name = "Quy đổi thành")]
		[Required]
		public string RedeemType { get; set; }
		[Display(Name = "Game")]
		public string? GameId { get; set; }
		public string? GameName { get; set; }
		public string RedeemedName { get; set; } // tên người sử dụng
		public string RemainingTime { get; set; }
		public GameRedeemCodeEditModel()
		{
			InputFields.Add<GameRedeemCodeEditModel>(c => c.Value);
			InputFields.Add<GameRedeemCodeEditModel>(c => c.Code);
			InputFields.Add<GameRedeemCodeEditModel>(c => c.QuantityCode);
			InputFields.Add<GameRedeemCodeEditModel>(c => c.GameId);


			DataSource[Property.NameProperty(c => c.RedeemType)] = Enum.GetValues(typeof(RedeemCodeType)).Cast<RedeemCodeType>()
				   .ToDictionary(c => c.ToString(), v => (ISelectItem)new SelectItem(v.ToString(), v.GetDescription()));
		}

		public override Dictionary<string, List<string>> Validate(string nameProperty)
		{
			var Errors = new Dictionary<string, List<string>>();
			if (nameProperty == Property.Name(c => c.Value))
			{
				if (RedeemType == RedeemCodeType.Money.ToString())
				{
					if (Value < 10000)
					{
						Errors.AddExist(nameProperty, "Giá trị mã nạp phải >= 10000");
					}
					if (Value > 500000)
					{
						Errors.AddExist(nameProperty, "Giá trị mã nạp phải <= 500000");
					}
				}
			}
			if (nameProperty == Property.Name(c => c.GameId))
			{
				if (RedeemType == RedeemCodeType.Game.ToString())
				{
					if (GameId.IsNullOrEmpty())
					{
						Errors.AddExist(nameProperty, "Vui lòng pick game đi");
					}
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
				if (GenerateType == GenerateRedeemType.Auto.ToString() && (QuantityCode <= 1 || QuantityCode >= 500))
				{
					Errors.AddExist(nameProperty, "Số lượng mã gen tự động tối thiểu = 2 và tối đa = 500"); // tránh spam
				}
			}
			return Errors;
		}
	}
}
