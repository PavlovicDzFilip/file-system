using FileSystem.Domain.Directories;
using System;

namespace FileSystem.Domain.Files
{
    public class File
    {
        public FileId Id { get; }
        public FileName Name { get; private set; }
        public DirectoryId ParentId { get; private set; }
        public DateTime CreatedAt { get; }
        public FileContent Content { get; }


        private File(FileId id, FileName name, DirectoryId parentId, DateTime createdAt, FileContent content)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
            CreatedAt = createdAt;
            Content = content;
        }

        internal File(DirectoryId parentId, FileName name, FileContent content):
            this(FileId.NewId(), name, parentId, DateTime.UtcNow, content)
        {
        }

        public void Rename(FileName fileName)
            => Name = fileName ?? throw new ArgumentNullException(nameof(fileName));

        public void SetParent(Directory parent)
            => ParentId = parent.Id;
    }
}