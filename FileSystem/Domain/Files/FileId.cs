using System;
using System.Diagnostics;
using System.Threading;

namespace FileSystem.Domain.Files
{
    [DebuggerDisplay("{Value}")]
    public class FileId
    {
        private static long _idCounter = DateTime.UtcNow.Ticks;
        public long Value { get; }

        public FileId(long value)
        {
            Value = value;
        }

        public static FileId NewId()
            => new(Interlocked.Increment(ref _idCounter));
    }
}