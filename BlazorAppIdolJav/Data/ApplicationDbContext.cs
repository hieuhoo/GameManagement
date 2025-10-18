using BlazorAppIdolJav.Share.Model.ClassModel;
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
    }
}

