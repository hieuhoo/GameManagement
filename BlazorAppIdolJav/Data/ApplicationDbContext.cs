using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Game { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<GameCompany> GameCompany { get; set; }
        public DbSet<GameType> GameType { get; set; }
        public DbSet<UserOtpHistory> UserOtpHistory { get; set; }
        public DbSet<UserPasswordHistory> UserPasswordHistory { get; set; }
        public DbSet<UserLockHistory> UserLockHistory { get; set; }
        public DbSet<GameDiscount> GameDiscount { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<GameRedeemCode> GameRedeemCode { get; set; }
        public DbSet<UserWallet> UserWallet { get; set; }
        public DbSet<WalletTransactionHistory> WalletTransactionHistory { get; set; }
        public DbSet<UserGameWishlist> UserGameWishlist { get; set; }
        public DbSet<UserGameLibrary> UserGameLibrary { get; set; }
        public DbSet<UserGameReview> UserGameReview { get; set; }


    }
}

