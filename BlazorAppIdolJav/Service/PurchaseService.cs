using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Service
{
	public class PurchaseService : IPurchaseService
	{
		private readonly ApplicationDbContext _context;
        //private readonly IUserWalletRepository _repo;
        private readonly IMapper _mapper;

        public PurchaseService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            //_repo = repo;
            _mapper = mapper;
        }

		public async Task<PurchaseGameResultData> ProcessPurchaseTransactionAsync(PaymentGameInforData data)
		{
			// khi pass qua bước check số dư sẽ gọi vào hàm này xử lý
			await using var transaction =
				await _context.Database.BeginTransactionAsync();

			try
			{
				var now = DateTime.Now;
				// cần lấy dữ liệu game
				// 1. Update wallet
				var wallet = await _context.UserWallet
					.FirstOrDefaultAsync(x => x.UserId == data.UserId);
				// còn 1 case nếu nhỡ 2 người cùng thao tác chưa check
				var balanceBefore = wallet.Balance;

				wallet.Balance -= data.CurrentPrice;
				wallet.UpdatedDate = now;

				// 2. Add library
				var item = new UserGameLibrary
				{
					Id = ObjectExtentions.GenerateGuid(),
					UserId = data.UserId,
					GameId = data.GameId,
					AcquireType = "Purchase", // hiện tại gán mặc định
					CreateDate = now
				};

				await _context.UserGameLibrary.AddAsync(item);


				// 3. Add transaction history
				var history = new WalletTransactionHistory
				{
					Id = ObjectExtentions.GenerateGuid(),
					UserId = data.UserId,
					Amount = -data.CurrentPrice,
					BalanceBefore = balanceBefore,
					BalanceAfter = wallet.Balance,
					Type = WalletTransactionType.PurchaseGame.ToString(),
					CreateDate = now,
                    PercentDiscount = data.PercentSale,
					OriginalGamePrice = data.OriginalPrice,
					PurchaseGamePrice = data.CurrentPrice,
                    ReferenceId = data.GameId, // hiện tại gán như này để nhận biết là game nào
					RedeemType = null // do đấy là purchase game ko phải dùng mã redeem
                };

				await _context.WalletTransactionHistory
					.AddAsync(history);

                // 4. Remove from wishlist
                var wishlistItem = await _context.UserGameWishlist
                    .FirstOrDefaultAsync(x =>
                        x.UserId == data.UserId &&
                        x.GameId == data.GameId);

                if (wishlistItem != null)
                {
                    _context.UserGameWishlist.Remove(wishlistItem);
                }

                await _context.SaveChangesAsync();

				await transaction.CommitAsync();
				return new PurchaseGameResultData
				{
					IsSuccess = true,
					TransactionId = item.Id,
					PurchaseDate = now.ToString(),
					DiscountPercent = data.PercentSale.ToString(),
					OriginalPrice = data.OriginalPrice.ToString(),
					PurchasePrice = data.CurrentPrice.ToString(),
					FullName = data.FullName,
					GameName = data.GameName
				};
			}
			catch
			{
				await transaction.RollbackAsync();
                return new PurchaseGameResultData
                {
                    IsSuccess = false
                };
            }
		}
    }
}
