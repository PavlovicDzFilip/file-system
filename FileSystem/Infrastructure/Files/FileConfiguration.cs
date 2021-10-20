using FileSystem.Domain.Directories;
using FileSystem.Domain.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileSystem.Infrastructure.Files
{
    public class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.HasKey(dir => dir.Id);
            builder
                .Property(dir => dir.Id)
                .IsFileId()
                .ValueGeneratedNever();

            builder
                .Property(dir => dir.ParentId)
                .IsDirectoryId();

            builder
                .HasOne<Directory>()
                .WithMany()
                .HasForeignKey(dir => dir.ParentId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasIndex(dir => new { dir.Name, dir.ParentId }, "IX_UniqueFileNameInDirectory")
                .IsUnique()
                .HasFilter(null);

            builder
                .Property(dir => dir.Name)
                .HasConversion(
                    name => name.Value,
                    value => FileName.Create(value));

            builder
                .Property(dir => dir.Content)
                .HasConversion(
                    content => content.Content, 
                    bytes => FileContent.Create(bytes));
        }
    }
}