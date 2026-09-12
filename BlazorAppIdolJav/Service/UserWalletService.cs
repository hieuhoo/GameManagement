using GameManagement.Data;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using Microsoft.EntityFrameworkCore;
using static GameManagement.Share.Extension.EnumExtension;
using GameManagement.Share.ClassDB;
using GameManagement.CoreConfig.Extensions;
namespace GameManagement.Service
{
	public class UserWalletService : IUserWalletService
	{
		private readonly ApplicationDbContext _context;

		public UserWalletService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<RedeemTransactionResultData> SaveRedeemFromCodeAsync(string userId, string code, string type) // type là Game hoặc money , game thì vào thư viện, money thì vào ví
		{
			await using var transaction =
				await _context.Database.BeginTransactionAsync();

			try
			{
				var now = DateTime.Now;
				// 1. CHECK REDEEM CODE

				var redeemCode = await _context
					.GameRedeemCode
					.FirstOrDefaultAsync(x => x.Code == code) ?? new GameRedeemCode();
				if (redeemCode == null)
				{
					await transaction.RollbackAsync();

					return new RedeemTransactionResultData
					{
						IsSuccess = false,
						Message = "Mã redeem ko tồn tại"
					};
				}

				if (redeemCode?.RedeemType != RedeemCodeType.Money.ToString()) // test hiện tại , sau tách logic hàm game riêng hàm tiền riêng
				{
					await transaction.RollbackAsync();

					return new RedeemTransactionResultData
					{
						IsSuccess = false,
						Message = "Đây ko phải mã redeem tiền"
					};
				}

				if (redeemCode?.Status != RedeemCodeStatus.Available.ToString())
				{
					await transaction.RollbackAsync();

					return new RedeemTransactionResultData
					{
						IsSuccess = false,
						Message = "Mã redeem đã bị khóa hoặc đã được sử dụng"
					};
				}
				if (redeemCode.ExpiredDate < now)
				{
					await transaction.RollbackAsync();

					return new RedeemTransactionResultData
					{
						IsSuccess = false,
						Message = "Mã redeem đã hết hạn"
					};
				}


				// 2. Check và tạo bản ghi cho wallet user
				decimal amount = Convert.ToDecimal(redeemCode?.Value.Value);
				var walletInfo = await _context.UserWallet.FirstOrDefaultAsync(x => x.UserId == userId);
				decimal balanceBefore;
				decimal balanceAfter;

				if (walletInfo == null) //chưa có thì tạo mới, có rồi thì update
				{
					balanceBefore = 0;
					balanceAfter = balanceBefore + amount;
					walletInfo = new UserWallet
					{
						Id = ObjectExtentions.GenerateGuid(),
						CreateDate = DateTime.Now,
						UpdatedDate = DateTime.Now,
						Balance = balanceAfter,
						UserId = userId,
					};
					await _context.UserWallet.AddAsync(walletInfo);
				}
				else
				{
					balanceBefore = walletInfo.Balance;
					balanceAfter = walletInfo.Balance + amount; //gán nó vào biến return sau cùng
					walletInfo.Balance = balanceAfter;
					walletInfo.UpdatedDate = DateTime.Now;
				}

				//3. Cập nhật status cho mã redeem code
				redeemCode.Status = RedeemCodeStatus.Used.ToString();
				redeemCode.RedeemedBy = userId;
				redeemCode.RedeemedDate = DateTime.Now;

				//4 . Thêm 1 record lịch sử transaction wallet
				var history = new WalletTransactionHistory
				{
					Id = ObjectExtentions.GenerateGuid(),
					CreateDate = DateTime.Now,
					UserId = userId,
					Type = WalletTransactionType.VoucherRedeem.ToString(),
					ReferenceId = code,
					Amount = amount,
					BalanceBefore = balanceBefore,
					BalanceAfter = balanceAfter,
				};
				await _context.WalletTransactionHistory.AddAsync(history);

				await transaction.CommitAsync();

				return new RedeemTransactionResultData {
					BalanceAfter = balanceAfter,
					Message = $"Bạn đã nạp thành công {amount} VNĐ từ mã redeem, số dư hiện tại của bạn là {balanceAfter} VNĐ",
					IsSuccess = true
				};
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}
	}
}
