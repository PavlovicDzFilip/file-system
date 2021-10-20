using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Domain.Files;
using Microsoft.EntityFrameworkCore;

namespace FileSystem.Infrastructure
{
    public class Database : DbContext, IUnitOfWork
    {
        public DbSet<Directory> Directories { get; set; }
        public DbSet<File> Files { get; set; }
        public Database(DbContextOptions<Database> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Database).Assembly);
        }

        public Task SaveChangesAsync()
            => base.SaveChangesAsync();
    }
}
