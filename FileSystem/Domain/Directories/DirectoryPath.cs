using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace FileSystem.Domain.Directories
{
    [DebuggerDisplay("{string.Join('/', System.Linq.Enumerable.Select(_directories, x => x.Name.Value))}")]
    public class DirectoryPath : IEnumerable<Directory>
    {
        private readonly ReadOnlyCollection<Directory> _directories;
        public static readonly DirectoryPath Empty = new(Array.Empty<Directory>());

        public DirectoryPath(IReadOnlyCollection<Directory> directories) :
            this(AddRoot(directories), true)
        {
        }

        private static IEnumerable<Directory> AddRoot(IEnumerable<Directory> directories)
        {
            var directoriesWithRoot = new List<Directory>(directories);
            directoriesWithRoot.Add(Directory.Root);
            return directoriesWithRoot;
        }

        private DirectoryPath(IEnumerable<Directory> directories, bool sortDirectories)
        {
            if (sortDirectories)
            {
                directories = SortDirectories(directories);
            }

            _directories = directories
                    .ToList()
                    .AsReadOnly();
        }

        private static IEnumerable<Directory> SortDirectories(IEnumerable<Directory> directories)
        {
            var uniqueDirectories = CreateHashset(directories);
            var parentDirectory = Directory.Root;

            var sorted = new List<Directory>(uniqueDirectories.Count);
            while (uniqueDirectories.Any())
            {
                sorted.Add(parentDirectory);
                uniqueDirectories.Remove(parentDirectory);

                if (uniqueDirectories.IsEmpty())
                {
                    break;
                }

                var childDirectories = uniqueDirectories
                    .Where(dir => dir.IsChildOf(parentDirectory))
                    .ToArray();

                if (childDirectories.IsNotSingle())
                {
                    throw new InvalidDirectoryHierarchyException();
                }

                parentDirectory = childDirectories.First();
            }

            return sorted.ToArray();
        }

        private static HashSet<Directory> CreateHashset(IEnumerable<Directory> directories)
        {
            var notUsedDirectories = new HashSet<Directory>(directories);
            if (notUsedDirectories.Count != directories.Count())
            {
                throw new DirectoriesNotUniqueException();

            }

            return notUsedDirectories;
        }

        public Directory Current
            => _directories.FirstOrDefault() ?? throw new Exception("Directory Path is empty");

        public DirectoryPath Next
            => new(_directories.Skip(1), false);

        public IEnumerator<Directory> GetEnumerator()
            => _directories.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)_directories).GetEnumerator();

        public bool TryMatchNext(Path path, out Directory directory)
        {
            directory = default;
            if (path.IsEmpty() || this.IsEmpty())
            {
                return false;
            }

            if (Current.Name.Equals(path.Current))
            {
                directory = Current;
                return true;
            }

            return false;
        }

        public Directory MatchFull(Path path)
        {
            bool isMatched = true;
            Directory directory = null;
            var directoryPath = this;
            while (isMatched && path.IsNotEmpty())
            {
                isMatched = directoryPath.TryMatchNext(path, out directory);
                path = path.Next;
                directoryPath = directoryPath.Next;
            }

            if (!isMatched)
            {
                return default;
            }

            return directory;
        }
    }
}