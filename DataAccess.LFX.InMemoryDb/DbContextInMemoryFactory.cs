using DataAccess.LFX.Core;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccess.LFX.InMemoryDb
{
    public class DbContextInMemoryFactory : DbContextFactoryAbstract
    {
        private readonly DbContextOptions<DbContextInMemory> options;
        public DbContextInMemoryFactory(string databaseName)
        {
            this.DatabaseName = databaseName;

            DbContextOptionsBuilder<DbContextInMemory> optionsBuilder = new DbContextOptionsBuilder<DbContextInMemory>();
            optionsBuilder.UseInMemoryDatabase(this.DatabaseName);

            this.options = optionsBuilder.Options;
        }

        public string DatabaseName { get; }

        public void SetupData(Action<DbContextInMemory> setupDataAction)
        {
            using (var context = this.CreateDbContext())
            {
                setupDataAction(context as DbContextInMemory);
            }
        }

        public override DbContextAbstract CreateDbContext()
        {
            return new DbContextInMemory(this.options);
        }
    }
}
