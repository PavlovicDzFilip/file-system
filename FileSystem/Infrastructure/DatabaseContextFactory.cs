using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FileSystem.Infrastructure
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<Database>
    {
        public Database CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Database>();
            optionsBuilder.UseSqlServer(@"User ID=sa;Password=****;Initial Catalog=FileSystem;Server=localhost");

            return new Database(optionsBuilder.Options);
        }
    }
}