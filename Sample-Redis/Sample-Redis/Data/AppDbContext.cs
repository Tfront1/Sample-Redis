using Microsoft.EntityFrameworkCore;
using Sample_Redis.Models;

namespace Sample_Redis.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {

        }

        public DbSet<Driver> Drivers { get; set; }
    }
}
