using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileSystem.Domain.Directories
{
    [DebuggerDisplay("{_path}")]
    public class Path : IEnumerable<DirectoryName>
    {
        private readonly ReadOnlyCollection<DirectoryName> _directoryNames;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly string _path;

        public static int MaxPathSize = 1000;

        private Path(DirectoryName[] directoryNames)
        {
            _directoryNames = directoryNames
                .ToList()
                .AsReadOnly();

            _path = string.Join('/', _directoryNames.Select(x => x.Value));
            if (_path.Length >= MaxPathSize)
            {
                throw new PathTooLongException();
            }
        }

        public static Path Parse(string path)
        {
            var directories = path
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(DirectoryName.Create)
                .ToArray();

            return new Path(directories);
        }

        public IEnumerator<DirectoryName> GetEnumerator()
        {
            return _directoryNames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _directoryNames.GetEnumerator();
        }

        public DirectoryName Current 
            => _directoryNames.FirstOrDefault() ?? throw new Exception("Path is empty");

        public Path Next
            => new(_directoryNames.Skip(1).ToArray());
    }
}