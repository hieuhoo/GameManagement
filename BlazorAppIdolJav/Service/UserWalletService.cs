using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;
using static GameManagement.Share.Extension.EnumExtension;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
namespace GameManagement.Service
{
    public class UserWalletService : IUserWalletService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserWalletRepository _repo;
        private readonly IMapper _mapper;

        public UserWalletService(ApplicationDbContext context, IUserWalletRepository repo, IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<UserWalletData>> GetAllWithFilterAsync(UserWalletSearch search)
        {
            try
            {
                var filter = search.CreateFilter(_repo.GetQueryable());
                var result = await _repo.GetAllWithFilterAsync(filter, search);
                var data = _mapper.Map<List<UserWalletData>>(result);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RedeemTransactionResultData> SaveMoneyFromTopupAsync(string userId, int money)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                var userWallet = await _context.UserWallet.FirstOrDefaultAsync(x => x.UserId == userId);
                UserWallet dataWallet = new UserWallet();
                decimal balanceAfter;
                // chưa có thì lưu mới
                if (userWallet == null)
                {
                    balanceAfter = money;
                    dataWallet = new UserWallet
                    {
                        Id = ObjectExtentions.GenerateGuid(),
                        CreateDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Balance = money,
                        UserId = userId,
                    };
                    await _context.UserWallet.AddAsync(dataWallet);

                    // lưu lịch sử transaction
                    var history = new WalletTransactionHistory
                    {
                        Id = ObjectExtentions.GenerateGuid(),
                        CreateDate = DateTime.Now,
                        UserId = userId,
                        Type = WalletTransactionType.Purchase.ToString(),
                        ReferenceId = ObjectExtentions.GenerateGuid(), // coi như đây là Id của giao dịch đó
                        Amount = money,
                        BalanceBefore = 0,
                        BalanceAfter = money,
                    };
                    await _context.WalletTransactionHistory.AddAsync(history);
                }
                else // có thì update
                {
                    var balanceBefore = userWallet.Balance;
                    balanceAfter = balanceBefore + money;

                    userWallet.UpdatedDate = DateTime.Now;
                    userWallet.Balance = balanceAfter;

                    // tạo recored lịch sử transaction
                    var history = new WalletTransactionHistory
                    {
                        Id = ObjectExtentions.GenerateGuid(),
                        CreateDate = DateTime.Now,
                        UserId = userId,
                        Type = WalletTransactionType.Purchase.ToString(),
                        ReferenceId = ObjectExtentions.GenerateGuid(), // coi như đây là Id của giao dịch đó
                        Amount = money,
                        BalanceBefore = balanceBefore,
                        BalanceAfter = balanceAfter,
                    };
                    await _context.WalletTransactionHistory.AddAsync(history);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new RedeemTransactionResultData
                {
                    IsSuccess = true,
                    BalanceAfter = balanceAfter,
                    
                    Message =  $"Bạn đã nạp thành công {money:N0} VNĐ từ giao dịch, " +
                               $"số dư hiện tại của bạn là {balanceAfter:N0} VNĐ"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<RedeemTransactionResultData> SaveRedeemFromCodeAsync(string userId, string code, string type) // type là Game hoặc money , game thì vào thư viện, money thì vào ví
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.Now;
                var currentBalance = await _context
                    .UserWallet
                    .FirstOrDefaultAsync(x => x.UserId == userId);
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
                        Message = "Mã redeem ko tồn tại",
                        BalanceBefore = currentBalance == null ? 0 : currentBalance.Balance
                    };
                }

                if (redeemCode?.RedeemType != RedeemCodeType.Money.ToString()) // test hiện tại , sau tách logic hàm game riêng hàm tiền riêng
                {
                    await transaction.RollbackAsync();

                    return new RedeemTransactionResultData
                    {
                        IsSuccess = false,
                        Message = "Đây ko phải mã redeem tiền",
                        BalanceBefore = currentBalance == null ? 0 : currentBalance.Balance
                    };
                }

                if (redeemCode?.Status != RedeemCodeStatus.Available.ToString())
                {
                    await transaction.RollbackAsync();

                    return new RedeemTransactionResultData
                    {
                        IsSuccess = false,
                        Message = "Mã redeem đã bị khóa hoặc đã được sử dụng",
                        BalanceBefore = currentBalance == null ? 0 : currentBalance.Balance
                    };
                }
                if (redeemCode.ExpiredDate < now)
                {
                    await transaction.RollbackAsync();

                    return new RedeemTransactionResultData
                    {
                        IsSuccess = false,
                        Message = "Mã redeem đã hết hạn",
                        BalanceBefore = currentBalance == null ? 0 : currentBalance.Balance
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

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new RedeemTransactionResultData
                {
                    BalanceAfter = balanceAfter,
                    Message = $"Bạn đã nạp thành công {amount:N0} VNĐ từ mã redeem, số dư hiện tại của bạn là {balanceAfter:N0} VNĐ",
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
