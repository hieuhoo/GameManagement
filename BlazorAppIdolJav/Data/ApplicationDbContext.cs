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

    }
}

