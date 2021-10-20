using FileSystem.Domain.Directories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileSystem.Infrastructure.Directories
{
    public class DirectoryConfiguration : IEntityTypeConfiguration<Directory>
    {
        public void Configure(EntityTypeBuilder<Directory> builder)
        {
            builder.HasKey(dir => dir.Id);
            builder
                .Property(dir => dir.Id)
                .IsDirectoryId()
                .IsRequired()
                .ValueGeneratedNever();

            builder
                .Property(dir => dir.ParentId)
                .IsDirectoryId();

            builder
                .HasMany<Directory>()
                .WithOne()
                .HasForeignKey(dir => dir.ParentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasIndex(dir => new { dir.Name, dir.ParentId }, "IX_UniqueNameInDirectory")
                .IsUnique()
                .HasFilter(null);

            builder
                .Property(dir => dir.Name)
                .HasConversion(
                    name => name.Value,
                    value => new DirectoryName(value));

            builder
                .Property(dir => dir.CreatedAt);
        }
    }
}