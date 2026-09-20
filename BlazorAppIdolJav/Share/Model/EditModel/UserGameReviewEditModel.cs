using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.SpecialComponent.ExtensionClass;
using System.ComponentModel.DataAnnotations;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Share.Model.EditModel
{
	public class UserGameReviewEditModel : EditBaseModel
	{
		public Property<UserGameReviewEditModel> Property { get; set; } = new Property<UserGameReviewEditModel>();

		public string Id { get; set; }

		[Display(Name = "Số sao")]
		[Required]
		public decimal StarNumber { get; set; }

		[Display(Name = "Cảm nghĩ của bạn")]
		[Required]
		public string Comment { get; set; }
		//public bool IsPositive { get; set; }
		public string UserId { get; set; }
		public string GameId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdatedDate { get; set; }

		[Display(Name = "Bạn có đề xuất không?")]
		public bool? IsRecommend { get; set; }

		public bool IsHidden { get; set; }
		public int LikeCount { get; set; } // tổng số like comment
		public int HeartCount { get; set; } // tổng số tym comment
        public int FunnyCount { get; set; } // tổng số like comment

        [Display(Name = "Bình luận dưới dạng ẩn danh ?")]
		[Required]
		public bool IsAnonymous { get; set; }

		public UserGameReviewEditModel()
		{
			InputFields.Add<UserGameReviewEditModel>(c => c.StarNumber);

		}

		public override Dictionary<string, List<string>> Validate(string nameProperty)
		{
			var Errors = new Dictionary<string, List<string>>();
			if (nameProperty == Property.Name(c => c.StarNumber))
			{
				if (StarNumber == 0)
				{
					Errors.AddExist(nameProperty, "Chưa vote số sao");
				}
			}
			return Errors;
		}

	}
}
