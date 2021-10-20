using FileSystem.Domain.Directories;
using FileSystem.Domain.Files;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileSystem.Infrastructure
{
    public static class IdExtensions
    {
        public static PropertyBuilder<DirectoryId> IsDirectoryId(this PropertyBuilder<DirectoryId> builder)
            => builder
                .HasConversion(
                    id => id.Value,
                    value => new DirectoryId(value));

        public static PropertyBuilder<FileId> IsFileId(this PropertyBuilder<FileId> builder)
            => builder
                .HasConversion(
                    id => id.Value,
                    value => new FileId(value))
                .IsRequired();
    }
}