using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FileSystem.Domain.Directories
{
    [DebuggerDisplay("{Name}")]
    public class Directory
    {
        public DirectoryId Id { get; }
        public DirectoryName Name { get; private set; }
        public DirectoryId ParentId { get; private set; }
        public DateTime CreatedAt { get; }

        public static readonly Directory Root = new(
            DirectoryId.NoParent,
            DirectoryName.RootDirectoryName,
            DirectoryId.NoParent,
            DateTime.MinValue.ToUniversalTime());

        private Directory(DirectoryId id, DirectoryName name, DirectoryId parentId, DateTime createdAt)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
            CreatedAt = createdAt;
        }

        public Directory(DirectoryId parentId, DirectoryName name)
        {
            Name = name;
            ParentId = parentId;
            Id = DirectoryId.NewId();
            CreatedAt = DateTime.UtcNow;
        }

        public bool IsChildOf(Directory directory)
            => this != Root && 
               ParentId == directory.Id;

        public IEnumerable<Directory> CreateChildren(Path path, DirectoryPath directoryPath)
        {
            if (path.IsEmpty())
            {
                return Array.Empty<Directory>();
            }

            var children = new List<Directory>();
            if (!directoryPath.TryMatchNext(path, out var child))
            {
                child = CreateChild(path.Current);
                children.Add(child);
            }

            var subChildren = child.CreateChildren(path.Next, directoryPath.Next);
            return children.Union(subChildren);
        }

        public IEnumerable<Directory> CreateChildren(Path path)
            => CreateChildren(path, DirectoryPath.Empty);

        public Directory CreateChild(DirectoryName name)
            => new(Id, name);

        public void Rename(DirectoryName directoryName)
            => Name = directoryName ?? throw new ArgumentNullException(nameof(directoryName));

        public void SetParent(Directory parent)
            => ParentId = parent.Id;
    }
}