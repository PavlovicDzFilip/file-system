using System;
using System.Diagnostics;
using System.Threading;

namespace FileSystem.Domain.Directories
{
    [DebuggerDisplay("{Value}")]
    public class DirectoryId
    {
        private static long _idCounter = DateTime.UtcNow.Ticks;
        public long Value { get; }

        public DirectoryId(long value)
        {
            Value = value;
        }

        public static DirectoryId NewId()
            => new(Interlocked.Increment(ref _idCounter));

        public static readonly DirectoryId NoParent = null;

        public override bool Equals(object obj)
            => obj is DirectoryId directoryId &&
               Equals(directoryId);

        protected bool Equals(DirectoryId other)
            => this == other;

        public override int GetHashCode()
            => Value.GetHashCode();

        public static bool operator ==(DirectoryId obj1, DirectoryId obj2)
            => ReferenceEquals(obj1, obj2) || (
                obj1 is not null &&
                obj2 is not null &&
                obj1.Value == obj2.Value);

        public static bool operator !=(DirectoryId obj1, DirectoryId obj2)
            => !(obj1 == obj2);
    }
}