using DataAccess.LFX.Core;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.LFX.InMemoryDb
{
    public class DbContextInMemory : DbContextAbstract
    {
        public DbContextInMemory()
        : base()
        {
        }

        public DbContextInMemory(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("GanshornInMemoryDb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TBD
        }

    }
}
