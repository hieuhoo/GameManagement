using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using GameManagement.Share.ClassData;
namespace GameManagement.WebInterface.User
{
    public partial class UserWallet : ComponentBase
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IGameRedeemCodeService RedeemService { get; set; }
        [Inject] IUserWalletService WalletService { get; set; }
        [Inject] NotificationService NoticeService { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }

        string? currentUserId;
        string? redeemCode = string.Empty;
        string cancelText = "Đóng";
        string currentBalance = string.Empty;
        bool isRedeemModalVisible;
        bool topUpFormVisible;
        string purchaseAmount = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await GetCurrentBalanceAsync();
            }
            catch
            {

            }
        }

        private readonly int[] WalletAmounts =
        {
            75000,
            150000,
            375000,
            750000,
            1500000
        };

        void OpenRedeemModal()
        {
            isRedeemModalVisible = true;
        }

        async Task EnterRedeemCodeAsync(string code)
        {
            try
            {
                code = code.Trim().ToUpperInvariant();
                if (code.IsNullOrEmpty())
                {
                    NoticeService.NotiWarning("Mã code không được bỏ trống");
                    return;
                }
                var dataRedeem = await RedeemService.GetAllWithFilterAsync(new GameRedeemSearch
                {
                    Code = code,
                }) ?? new List<Share.ClassData.GameRedeemCodeData>();
                var typeRedeemCode = dataRedeem?.FirstOrDefault()?.RedeemType;
                var result = await WalletService.SaveRedeemFromCodeAsync(currentUserId, code, typeRedeemCode);
                //viết logic : 1.check mã code (status, update ngược vào bảng,... gameredeemcode)
                // 2. Check nếu là giao dịch đầu thì tạo 1 record wallet, có thì update : id, userid, balance (số dư hiện tại),updatedate.createdate
                // 3. Thêm 1 record vào bảng WalletTransactionHistory : id, userid, amount (kiểu decimal), type (RedeemVoucher/Purchase) , balancebefore,
                //        balanceAfter (Sau = trước + amount), createdate, referenceId (là mã code nhập hoặc Id giao dịch hoặc mã id game nếu là mua game) 
                // Tất cả phải trong cùng 1 transaction
                if (result.IsSuccess)
                {
                    NoticeService.NotiSuccess(result.Message);
                    currentBalance = result.BalanceAfter.ToString("N0");
                    isRedeemModalVisible = false;
                    redeemCode = null;
                }
                else
                {
                    NoticeService.NotiWarning(result.Message);
                }
            }
            catch
            {

            }
        }

        void CloseRedeemForm()
        {
            isRedeemModalVisible = false;
        }

        async Task GetCurrentBalanceAsync()
        {
            try
            {
                var data = await WalletService.GetAllWithFilterAsync(new UserWalletSearch
                {
                    UserId = currentUserId
                }) ?? new List<UserWalletData>();
                var balance = data?.FirstOrDefault()?.Balance ?? 0;

                currentBalance = balance.ToString("N0");
            }
            catch
            {

            }
        }

        void OpenTopUpForm(int valueMoney)
        {
            purchaseAmount = valueMoney.ToString("N0");
            topUpFormVisible = true;

        }

        void CloseTopUpForm()
        {
            topUpFormVisible = false;
        }

        async Task PurchaseAsync(string value)
        {
            try
            {
                int money = int.Parse(value.Replace(",", ""));
                var result = await WalletService.SaveMoneyFromTopupAsync(currentUserId, money);
                if (result.IsSuccess)
                {
                    NoticeService.NotiSuccess(result.Message);
                }
                else
                {
                    NoticeService.NotiError(result.Message);
                }
                currentBalance = result.BalanceAfter.ToString("N0");
                topUpFormVisible = false;
            }
            catch
            {

            }
        }
    }
}
