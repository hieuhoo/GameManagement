using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.EditModel;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.GameInformation
{
	public partial class GameDetail : ComponentBase
	{
		[Inject] IMapper Mapper { get; set; }
		[Inject] IGameService GameService { get; set; }
		[Inject] NotificationService NoticeService { get; set; }
		[Parameter] public EventCallback<int> ReSize { get; set; }
		[Parameter] public List<GameCompanyData> CompanyDatas { get; set; }
		[Parameter] public List<GameTypeData> GameTypeDatas { get; set; }
		[Parameter] public EventCallback OnCancel { get; set; }
		[Parameter] public EventCallback ReloadGameData { get; set; }
		[Parameter] public int TotalCurrentFeatureGame { get; set; }
		[Parameter] public List<GameData> FeatureDatas { get; set; }

		GameEditModel EditModel { get; set; } = new();
		List<SelectItem> NationalOptions { get; set; } = new();
		List<SelectItem> GameReleaseStatusOptions { get; set; } = new();
		List<SelectItem> GameSoldStatusOptions { get; set; } = new();
		List<SelectItem> PlatformOptions { get; set; } = new();
		List<SelectItem> GameStatusOptions { get; set; } = new();
		List<SelectItem> WindowOptions { get; set; } = new();
		List<SelectItem> CpuOptions { get; set; } = new();


		List<UploadFileItem> IdentityTemplateFiles { get; set; } = new();

		int Size => (EditModel.ImagePath.IsNotNullOrEmpty())
						  ? 12 : 0;

		IList<IBrowserFile> TemplateBrowserFiles = new List<IBrowserFile>();
		InputWatcher inputWatcher;
		string idCardUpload = null;
		string tempIdentityPathFile;
		string game = "GameURL";
		string? ImagePreviewUrl;
		byte[]? ImageBytes;
		int? previousOrder;
		bool  originalIsFeatured;
		protected override async Task OnInitializedAsync()
		{
			try
			{
				EditModel = new GameEditModel();
				EditModel.ReadOnly = false;
				idCardUpload = ObjectExtentions.GenerateGuid();
				NationalOptions = Enum.GetValues(typeof(National)).Cast<National>()
					 .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
				GameReleaseStatusOptions = Enum.GetValues(typeof(GameReleaseStatus)).Cast<GameReleaseStatus>()
					.Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
				GameSoldStatusOptions = Enum.GetValues(typeof(GameSaleStatus)).Cast<GameSaleStatus>()
				   .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
				PlatformOptions = Enum.GetValues(typeof(PlatformSystem)).Cast<PlatformSystem>()
				   .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
				GameStatusOptions = Enum.GetValues(typeof(GameActiveStatus)).Cast<GameActiveStatus>()
				.Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
				WindowOptions = Enum.GetValues(typeof(WindowConfig)).Cast<WindowConfig>()
				.Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
				CpuOptions = Enum.GetValues(typeof(CPUConfig)).Cast<CPUConfig>()
				.Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		async Task ReadImageAsync(InputFileChangeEventArgs e)
		{
			try
			{
				var file = e.File;

				TemplateBrowserFiles.Clear();
				TemplateBrowserFiles.Add(file);

				IdentityTemplateFiles = TemplateBrowserFiles
					.Select(x => new UploadFileItem
					{
						FileName = x.Name,
						Size = x.Size
					})
					.ToList();

				// Đọc ảnh vào memory
				await using var stream = file.OpenReadStream(GlobalVariant.MaxFileSize);
				using var memoryStream = new MemoryStream();
				await stream.CopyToAsync(memoryStream);

				ImageBytes = memoryStream.ToArray();
				// Preview ngay trên UI
				ImagePreviewUrl =
					$"data:{file.ContentType};base64,{Convert.ToBase64String(ImageBytes)}";

				EditModel.ImageName = file.Name;

				StateHasChanged();
			}
			catch
			{
				throw;
			}
		}

		string AttachPath(string character, string baseFolder = null, string fileName = null)
		{
			var path = Path.Combine(baseFolder, character, "Files", "Game", "Image", "Attach");
			if (fileName.IsNotNullOrEmpty())
			{
				path = Path.Combine(path, fileName);
			}
			return path;
		}

		void RemoveImage()
		{
			ImagePreviewUrl = null;
			ImageBytes = null;

			EditModel.ImageName = string.Empty;
			EditModel.ImagePath = string.Empty;

			TemplateBrowserFiles.Clear();
			IdentityTemplateFiles?.Clear();
			StateHasChanged();
		}

		void OnChangeFeatured(bool value)
		{
			EditModel.IsFeatured = value;
			//case đang nổi bật => bỏ tick xong tick lại cần bind vào
			if (!value)
			{
				previousOrder = EditModel.OrderFeatured;
				EditModel.OrderFeatured = null;
			}
			else
			{
				EditModel.OrderFeatured = previousOrder;
			}
		}

		void CheckChangeStatus(string value)
		{
			if (value == GameActiveStatus.Inactive.ToString())
			{
				EditModel.IsFeatured = false;
				EditModel.OrderFeatured = null;
			}
		}

		async Task SaveAsync()
		{
			try
			{
				if (EditModel.Id.IsNullOrEmpty())
				{
					await CreateAsync();
				}
				else
				{
					await UpdateAsync();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		async Task CancelAsync()
		{
			try
			{
				EditModel = new GameEditModel();
				await OnCancel.InvokeAsync();
			}
			catch
			{

			}
		}

		async Task CreateAsync()
		{
			try
			{
				var errorMessageStore = EditModel.ValidateAll();
				if (!inputWatcher.Validate() || errorMessageStore?.Any() == true)
				{
					if (errorMessageStore.Any())
					{
						inputWatcher.NotifyFieldChanged(errorMessageStore.First().Key, errorMessageStore);
					}
					NoticeService.NotiWarning(TypeAlert.InvalidData.GetDescription());
					return;
				}
				EditModel.Id = ObjectExtentions.GenerateGuid();
				EditModel.CreateDate = DateTime.Now;
				if (ImageBytes != null &&
					!string.IsNullOrWhiteSpace(EditModel.ImageName))
				{
					var extension = Path.GetExtension(EditModel.ImageName);
					var fileName = $"{ObjectExtentions.GenerateGuid()}{extension}";
					// Relative path sẽ lưu DB
					var relativePath = Path.Combine(
						game,
						fileName
					);
					// Physical path để ghi file thật
					var physicalPath = Path.Combine(
						GlobalVariant.UploadFolder,
						relativePath
					);
					var directory = Path.GetDirectoryName(physicalPath);
					if (!Directory.Exists(directory))
					{
						Directory.CreateDirectory(directory!);
					}
					await File.WriteAllBytesAsync(
						physicalPath,
						ImageBytes
					);
					EditModel.ImageName = fileName;
					EditModel.ImagePath = relativePath;

					EditModel.IsInUnsafeUpload = false;
				}
				var data = Mapper.Map<GameData>(EditModel);
				var isSucess = await GameService.SaveGameAsync(data);
				if (isSucess)
				{
					NoticeService.NotiSuccess(OperationEnum.AddSuccessfully.GetDescription());
					EditModel = new();
					await OnCancel.InvokeAsync();
					await ReloadGameData.InvokeAsync();
				}
				else
				{
					NoticeService.NotiError(OperationEnum.AddFailed.GetDescription());
				}
			}
			catch
			{

			}
		}

		async Task UpdateAsync()
		{
			string? oldImagePath = null;
			string? newPhysicalPath = null;

			try
			{
				var errorMessageStore = EditModel.ValidateAll();

				if (!inputWatcher.Validate() ||
					errorMessageStore?.Any() == true)
				{
					if (errorMessageStore?.Any() == true)
					{
						inputWatcher.NotifyFieldChanged(
							errorMessageStore.First().Key,
							errorMessageStore
						);
					}
					NoticeService.NotiWarning(
						TypeAlert.InvalidData.GetDescription()
					);
					return;
				}

				oldImagePath = EditModel.ImagePath;

				// 3. NẾU USER CHỌN ẢNH MỚI
				if (ImageBytes != null &&
					ImageBytes.Length > 0 &&
					!string.IsNullOrWhiteSpace(EditModel.ImageName))
				{
					var extension = Path.GetExtension(EditModel.ImageName);
					var fileName = $"{ObjectExtentions.GenerateGuid()}{extension}";
					// Path lưu DB
					var relativePath =
						Path.Combine(
							game,
							fileName
						);
					// Path file thật
					var physicalPath =
						Path.Combine(
							GlobalVariant.UploadFolder,
							relativePath
						);
					var directory = Path.GetDirectoryName(physicalPath);
					if (!Directory.Exists(directory))
					{
						Directory.CreateDirectory(directory!);
					}
					// Lưu ảnh mới
					await File.WriteAllBytesAsync(
						physicalPath,
						ImageBytes
					);

					newPhysicalPath = physicalPath;

					// Update thông tin ảnh mới vào model
					EditModel.ImageName = fileName;
					EditModel.ImagePath = relativePath;

					EditModel.IsInUnsafeUpload = false;
				}
				var data = Mapper.Map<GameData>(EditModel);
		
				var isSuccess = await GameService.UpdateGameAsync(data);

				if (isSuccess)
				{
					// 6. XÓA ẢNH CŨ NẾU CÓ ẢNH MỚI
					if (ImageBytes != null &&
						ImageBytes.Length > 0 &&
						!string.IsNullOrWhiteSpace(oldImagePath))
					{
						var oldPhysicalPath =
							Path.Combine(
								GlobalVariant.UploadFolder,
								oldImagePath
							);

						if (File.Exists(oldPhysicalPath))
						{
							File.Delete(oldPhysicalPath);
						}
					}
					NoticeService.NotiSuccess(
						OperationEnum
							.UpdateSuccessfully
							.GetDescription()
					);
					EditModel = new();
					ImageBytes = null;
					await OnCancel.InvokeAsync();
					await ReloadGameData.InvokeAsync();
				}
				else
				{
					// DB UPDATE FAIL
					// XÓA ẢNH MỚI VỪA UP
					if (!string.IsNullOrWhiteSpace(newPhysicalPath) &&
						File.Exists(newPhysicalPath))
					{
						File.Delete(newPhysicalPath);
					}

					NoticeService.NotiError(
						OperationEnum
							.UpdateFailed
							.GetDescription()
					);
				}
			}
			catch (Exception ex)
			{
				// Nếu có ảnh mới nhưng update lỗi
				// thì xóa file mới để tránh file rác
				if (!string.IsNullOrWhiteSpace(newPhysicalPath) &&
					File.Exists(newPhysicalPath))
				{
					File.Delete(newPhysicalPath);
				}
				NoticeService.NotiError(
					$"Cập nhật thất bại: {ex.Message}"
				);
			}
		}

		bool DisableCheckboxFeature()
		{
			return TotalCurrentFeatureGame >= 3 && !EditModel.IsFeatured  && !originalIsFeatured;
		}

		public async Task LoadEditModelAsync(GameViewModel model)
		{
			var data = await GameService.GetAllWithFilterAsync(new GameSearch
			{
				Id = model.Id
			});
			var result = data.Where(c => c.Id == model.Id).FirstOrDefault(); // code hơi cấn
			EditModel = Mapper.Map<GameEditModel>(result);
			originalIsFeatured = EditModel.IsFeatured;
		}

		bool IsOrderDisabled(int order)
		{
			return FeatureDatas.Any(x =>
				x.Id != EditModel.Id &&
				x.OrderFeatured == order);
		}

	}
}
