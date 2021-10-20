using System;
using System.Diagnostics;

namespace FileSystem.Domain.Files
{
    [DebuggerDisplay("{Value}")]
    public class FileName : IEquatable<FileName>
    {
        public string Value { get; }
        private static readonly StringComparer ValueComparer = StringComparer.InvariantCultureIgnoreCase;

        private FileName(string value)
        {
            Value = value;
        }

        public static FileName Create(string value)
        {
            value = value?.ToLowerInvariant();
            var name = System.IO.Path.GetFileNameWithoutExtension(value);
            var extension = System.IO.Path.GetExtension(value);

            return new FileName(value);
        }

        public bool Equals(FileName other)
            => ValueComparer.Equals(Value, other.Value);

        public override bool Equals(object obj)
            => obj is FileName fileName && Equals(fileName);

        public override int GetHashCode()
           => ValueComparer.GetHashCode(Value);
    }
}