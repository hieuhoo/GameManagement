using BlazorAppIdolJav.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppIdolJav.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Actress> Actress { get; set; }
        public DbSet<User> User { get; set; }
    }
}

