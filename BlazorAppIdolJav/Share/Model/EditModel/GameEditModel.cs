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
		public string Status { get; set; } = GameActiveStatus.Active.ToString();

		[Display(Name = "Trạng thái bán hàng")]
		[Required]
		public string SoldStatus { get; set; }

		[Display(Name = "Giá tiền")]
		[Required]
		public int Price { get; set; }

		[Display(Name = "Đơn vị tiền")]
		[Required]
		public string Unit { get; set; }

		[Display(Name = "Mô tả")]
        [Required]
        public string Description { get; set; }

		[Display(Name = "Nền tảng hỗ trợ")]
		[Required]
		public string SystemSupport { get; set; }

		[Display(Name = "Thuộc hãng game")]
		[Required]
		public string GameCompanyId { get; set; }

		[Display(Name = "Thể loại game")]
		[Required]
		public IEnumerable<string> GameTypeId { get; set; }
			= new List<string>();

		[Display(Name = "Trạng thái phát hành")]
		[Required]
		public string ReleaseStatus { get; set; }

		[Display(Name = "Game nổi bật")]
		public bool IsFeatured { get; set; }

		[Display(Name = "Thứ tự nổi bật")]
		public int? OrderFeatured { get; set; }

		[Display(Name = "Window tối thiểu")]
		[Required]
		public string? MinOS { get; set; } //win 10  -11-12

		[Display(Name = "CPU tối thiểu")]
		[Required]
		public string? MinCPU { get; set; }

		[Display(Name = "RAM tối thiểu (GB)")]
		[Required]
		public int? MinRAM { get; set; } // option 8 16 32

		[Display(Name = "GPU tối thiểu")]
		[Required]
		public string? MinGPU { get; set; }

		[Display(Name = "Window đề xuất")]
		[Required]
		public string? RecommendOS { get; set; }

		[Display(Name = "CPU đề xuất")]
		[Required]
		public string? RecommendCPU { get; set; }

		[Display(Name = "RAM đề xuất (GB)")]
		[Required]
		public int? RecommendRAM { get; set; }

		[Display(Name = "GPU đề xuất")]
		[Required]
		public string? RecommendGPU { get; set; }

		[Display(Name = "Direct yêu cầu")]
		[Required]
		public string? DirectX { get; set; }

		[Display(Name = "Dung lượng cần có")]
		[Required]
		public int? StorageRequired { get; set; } // GB

		[Display(Name = "Ghi chú khác")]
		public string? SystemNote { get; set; } //ghi chú
		public GameEditModel()
		{
			InputFields.Add<GameEditModel>(c => c.Price);
			InputFields.Add<GameEditModel>(c => c.OrderFeatured);
			InputFields.Add<GameEditModel>(c => c.RecommendRAM);
			InputFields.Add<GameEditModel>(c => c.StorageRequired);


			DataSource[Property.NameProperty(c => c.Unit)] = Enum.GetValues(typeof(UnitMoneyEnum)).Cast<UnitMoneyEnum>()
				   .ToDictionary(c => c.ToString(), v => (ISelectItem)new SelectItem(v.ToString(), v.GetDescription()));
			
			DataSource[Property.NameProperty(c => c.MinOS)] = Enum.GetValues(typeof(WindowConfig)).Cast<WindowConfig>()
				   .ToDictionary(c => c.ToString(), v => (ISelectItem)new SelectItem(v.ToString(), v.GetDescription()));

			DataSource[Property.NameProperty(c => c.RecommendOS)] = Enum.GetValues(typeof(WindowConfig)).Cast<WindowConfig>()
				   .ToDictionary(c => c.ToString(), v => (ISelectItem)new SelectItem(v.ToString(), v.GetDescription()));

			DataSource[Property.NameProperty(c => c.MinCPU)] = Enum.GetValues(typeof(CPUConfig)).Cast<CPUConfig>()
				   .ToDictionary(c => c.ToString(), v => (ISelectItem)new SelectItem(v.ToString(), v.GetDescription()));

			DataSource[Property.NameProperty(c => c.RecommendCPU)] = Enum.GetValues(typeof(CPUConfig)).Cast<CPUConfig>()
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
			if (nameProperty == Property.Name(c => c.OrderFeatured))
			{
				if (IsFeatured == true && OrderFeatured == null)
				{
					Errors.AddExist(nameProperty, "Vui lòng chọn thứ tự nổi bật của game");
				}
			}
			if (nameProperty == Property.Name(c => c.StorageRequired))
			{
				if (StorageRequired <= 0)
				{
					Errors.AddExist(nameProperty, "Dung lượng yêu cầu phải lớn hơn 0");
				}
			}
			if (nameProperty == Property.Name(c => c.RecommendRAM))
			{
				if (RecommendRAM != null && MinRAM != null && RecommendRAM < MinRAM)
				{
					Errors.AddExist(nameProperty, "RAM đề xuất phải lớn hơn RAM tối thiểu");
				}
			}
			return Errors;
		}

	}
}
