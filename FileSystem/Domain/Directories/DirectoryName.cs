using System;
using System.Diagnostics;
using System.Linq;

namespace FileSystem.Domain.Directories
{
    [DebuggerDisplay("{Value}")]
    public class DirectoryName : IEquatable<DirectoryName>
    {
        public string Value { get; }
        public const int MaxLength = 100;

        private static readonly StringComparer ValueComparer = StringComparer.InvariantCultureIgnoreCase;
        public static readonly DirectoryName RootDirectoryName = new(string.Empty);

        public DirectoryName(string value)
        {
            Value = value;
        }

        public static DirectoryName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                !value.All(char.IsLetterOrDigit) ||
                value.Length > MaxLength)
            {
                throw new ArgumentException("Invalid directory name", nameof(value));
            }

            value = value.ToLowerInvariant();
            return new DirectoryName(value);
        }

        public bool Equals(DirectoryName other)
            => ValueComparer.Equals(Value, other.Value);

        public override bool Equals(object obj)
            => Equals(obj as DirectoryName);

        public override int GetHashCode()
            => ValueComparer.GetHashCode(Value);
    }
}