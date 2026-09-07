using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.EditModel;
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
		GameEditModel EditModel { get; set; } = new();
		List<SelectItem> NationalOptions { get; set; } = new();
		List<SelectItem> GameReleaseStatusOptions { get; set; } = new();
		List<SelectItem> GameSoldStatusOptions { get; set; } = new();
		List<SelectItem> PlatformOptions { get; set; } = new();
		List<SelectItem> GameStatusOptions { get; set; } = new();

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

			if (!value)
			{
				EditModel.OrderFeatured = null;
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
					var fileName = 	$"{ObjectExtentions.GenerateGuid()}{extension}";
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

		}
	}
}
