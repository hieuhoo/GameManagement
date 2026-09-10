using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.EditModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using OneOf.Types;
using System.Text.RegularExpressions;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.GameInformation
{
    public partial class GameRedeemCodeDetail : ComponentBase
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IGameRedeemCodeService RedeemService { get; set; }
        [Inject] NotificationService NoticeService { get; set; }

        GameRedeemCodeEditModel EditModel { get; set; } = new();
        InputWatcher inputWatcher;

        bool redeemVisible;
        bool saving;

        void ChangeTypeGenCode(string type)
        {
            if (type == GenerateRedeemType.ByHand.ToString())
            {
                EditModel.QuantityCode = null;
            }
        }

        void CloseRedeemDetail()
        {
            redeemVisible = false;
            EditModel = new GameRedeemCodeEditModel();
        }

        public async Task OpenRedeemForm()
        {
            try
            {
                redeemVisible = true;
                await InvokeAsync(StateHasChanged);
            }
            catch
            {

            }
        }

        async Task SaveRedeemAsync()
        {
            try
            {
                saving = true;
                if (EditModel.Id.IsNullOrEmpty())
                {
                    await CreateAsync();
                }
                else
                {
                    await UpdateAsync();
                }
            }
            catch
            {

            }
            finally
            {
                saving = false;
            }
        }

        string GenerateRedeemCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();

            var suffix = new string(
                Enumerable.Range(0, 3)
                    .Select(_ => chars[random.Next(chars.Length)])
                    .ToArray());

            return $"{GlobalVariant.PrefixRedeemCode}{suffix}";
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
                var prefix = GlobalVariant.PrefixRedeemCode;
                if (EditModel.GenerateType == GenerateRedeemType.ByHand.ToString())
                {
                    EditModel.Code = EditModel.Code.Trim().ToUpper();

                    if (!Regex.IsMatch(
                        EditModel.Code ?? "",
                        $"^{Regex.Escape(prefix)}[A-Z0-9]{{3}}$"))
                    {
                        NoticeService.NotiWarning(
                            $"Mã nạp phải có dạng {prefix}XXX.");

                        return;
                    }
                    EditModel.Id = ObjectExtentions.GenerateGuid();
                    EditModel.CreateDate = DateTime.Now;
                    EditModel.CreateBy = "ADMIN"; //TEST
                    EditModel.Status = RedeemCodeStatus.Active.ToString();
                    var redeemData = Mapper.Map<GameRedeemCodeData>(EditModel);
                    var isSuccess = await RedeemService.SaveRedeemAsync(redeemData);
                    if (isSuccess)
                    {
                        NoticeService.NotiSuccess(OperationEnum.AddSuccessfully.GetDescription());
                    }
                    else
                    {
                        NoticeService.NotiError(OperationEnum.AddFailed.GetDescription());
                    }
                    EditModel = new GameRedeemCodeEditModel();
                    redeemVisible = false;
                    return;
                }
                else
                {
                    var listRedeem = new List<GameRedeemCodeData>();
                    var batchId = ObjectExtentions.GenerateGuid();
                    for (int x = 0; x < EditModel.QuantityCode; x++)
                    {
                        var item = new GameRedeemCodeData
                        {
                            Id = ObjectExtentions.GenerateGuid(),
                            Code = GenerateRedeemCode(),
                            Value = EditModel.Value,
                            CreateDate = DateTime.Now,
                            CreateBy = "ADMIN",
                            Status = RedeemCodeStatus.Active.ToString(),
                            ExpiredDate = EditModel.ExpiredDate,
                            BatchId = batchId,
                            GenerateType = GenerateRedeemType.Auto.ToString()
                        };
                        listRedeem.Add(item);
                    }
                    var isSuccess = await RedeemService.SaveListRedeemAsync(listRedeem);
                    if (isSuccess)
                    {
                        NoticeService.NotiSuccess(OperationEnum.AddSuccessfully.GetDescription());
                    }
                    else
                    {
                        NoticeService.NotiError(OperationEnum.AddFailed.GetDescription());
                    }
                    EditModel = new GameRedeemCodeEditModel();
                    redeemVisible = false;
                    return;
                }
            }
            catch
            {

            }
        }

        async Task UpdateAsync()
        {
            try
            {

            }
            catch
            {

            }
        }
    }
}
