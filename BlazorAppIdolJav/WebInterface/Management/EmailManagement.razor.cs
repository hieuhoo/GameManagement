using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.ViewModel;
using Microsoft.AspNetCore.Components;

namespace GameManagement.WebInterface.Management
{
	public partial class EmailManagement : ComponentBase
	{
		[Inject] IEmailService MailService { get; set; }
		[Inject] IMapper Mapper { get; set; }

		List<EmailInformationViewModel> ViewModels { get; set; } = new List<EmailInformationViewModel>();
		List<EmailInformationData> MailDatas { get; set; } = new List<EmailInformationData>();
		EmailInformationViewModel MailDetail { get; set; } = new EmailInformationViewModel();
		Table<EmailInformationViewModel> Table;

		bool loading;
		bool mailVisible;
		int width;
		int height;
		string title = string.Empty;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				width = ConfigTemplate.Width / 5 * 4;
				height = ConfigTemplate.Height;
				await LoadInformationAsync();
			}
			catch
			{

			}
		}

		async Task LoadInformationAsync()
		{
			try
			{
				MailDatas = await MailService.GetAllWithFilterAsync(new MailSearch
				{

				});
				ViewModels = Mapper.Map<List<EmailInformationViewModel>>(MailDatas);
				ViewModels.OrderByDescending(c => c.CreateDate).ToList();
				int stt = 1;
				foreach (var item in ViewModels)
				{
					item.Stt = stt++;
				}
			}
			catch
			{

			}
		}

		void CloseDetailMail()
		{
			mailVisible = false;
		}


		async Task ViewDetailMailAsync(EmailInformationViewModel view)
		{
			try
			{
				MailDetail = view;
				title = "Thông tin email gửi";
				mailVisible = true;
			}
			catch
			{

			}
		}

		void OnRowClick(RowData<EmailInformationViewModel> rowData)
		{
			try
			{

			}
			catch
			{

			}
			StateHasChanged();
		}


	}
}
