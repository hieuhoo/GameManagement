using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Share.Model.EditModel
{
	public class GameDiscountEditModel : EditBaseModel
	{
		public Property<GameDiscountEditModel> Property { get; set; } = new Property<GameDiscountEditModel>();

		public string Id { get; set; }

		[Display(Name = "Game giảm giá")]
		[Required]
		public string GameId { get; set; }

		[Display(Name = "% giảm")]
		[Required]
		public int DiscountPercent { get; set; }

		[Display(Name = "Thời gian bắt đầu")]
		[Required]
		public DateTime? StartDate { get; set; }

		[Display(Name = "Thời gian kết thúc")]
		[Required]
		public DateTime? EndDate { get; set; }

		[Display(Name = "Trạng thái")]
		[Required]
		public string Status { get; set; }

		public DateTime CreateDate { get; set; }
		public GameDiscountEditModel()
		{
			InputFields.Add<GameDiscountEditModel>(c => c.DiscountPercent);
			InputFields.Add<GameDiscountEditModel>(c => c.StartDate);
			InputFields.Add<GameDiscountEditModel>(c => c.EndDate);

		}

		public override Dictionary<string, List<string>> Validate(string nameProperty)
		{
			var Errors = new Dictionary<string, List<string>>();

			if (nameProperty == Property.Name(c => c.DiscountPercent))
			{
				if (DiscountPercent < 10)
				{
					Errors.AddExist(nameProperty, "% giảm tối thiểu là 10%");
				}

				if (DiscountPercent > 85)
				{
					Errors.AddExist(nameProperty, "% giảm tối đa là 85%");
				}
			}

			if (nameProperty == Property.Name(c => c.StartDate))
			{
				if (StartDate.HasValue && EndDate.HasValue)
				{
					if (StartDate.Value > EndDate.Value)
					{
						Errors.AddExist(
							nameProperty,
							"Ngày bắt đầu không được lớn hơn ngày kết thúc"
						);
					}

					if (EndDate.Value > StartDate.Value.AddDays(10))
					{
						Errors.AddExist(
							nameProperty,
							"Thời gian discount không được vượt quá 10 ngày"
						);
					}
				}
			}

			if (nameProperty == Property.Name(c => c.EndDate))
			{
				if (StartDate.HasValue && EndDate.HasValue)
				{
					if (EndDate.Value < StartDate.Value)
					{
						Errors.AddExist(
							nameProperty,
							"Ngày kết thúc không được nhỏ hơn ngày bắt đầu"
						);
					}

					if (EndDate.Value > StartDate.Value.AddDays(10))
					{
						Errors.AddExist(
							nameProperty,
							"Thời gian discount không được vượt quá 10 ngày"
						);
					}
				}
			}

			return Errors;
		}
	}
}
