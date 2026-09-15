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

		async Task<PurchaseGameResultData> ProcessPurchaseTransactionAsync( // truyền vào object vì là mua game : % , giá, giá gốc
			string userId,
			string gameId,
			decimal purchasePrice)
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
					.FirstOrDefaultAsync(x => x.UserId == userId);
				// còn 1 case nếu nhỡ 2 người cùng thao tác chưa check
				var balanceBefore = wallet.Balance;

				wallet.Balance -= purchasePrice;
				wallet.UpdatedDate = now;

				// 2. Add library
				var item = new UserGameLibrary
				{
					Id = ObjectExtentions.GenerateGuid(),
					UserId = userId,
					GameId = gameId,
					AcquireType = "Purchase", // hiện tại gán mặc định
					CreateDate = now
				};

				await _context.UserGameLibrary.AddAsync(item);


				// 3. Add transaction history
				var history = new WalletTransactionHistory
				{
					Id = ObjectExtentions.GenerateGuid(),
					UserId = userId,
					Amount = -purchasePrice,
					BalanceBefore = balanceBefore,
					BalanceAfter = wallet.Balance,
					Type = WalletTransactionType.PurchaseGame.ToString(),
					ReferenceId = library.Id,
					CreateDate = now
				};

				await _context.WalletTransactionHistories
					.AddAsync(history);


				await _context.SaveChangesAsync();

				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}
	}
}
