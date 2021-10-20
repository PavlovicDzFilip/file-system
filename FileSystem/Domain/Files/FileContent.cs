using System;

namespace FileSystem.Domain.Files
{
    public class FileContent
    {
        public byte[] Content { get; }

        private FileContent(byte[] content)
        {
            Content = content;
        }

        public static FileContent Create(byte[] data)
        {
            if (data is not null &&
                data.IsNotEmpty())
            {
                return new FileContent(data);
            }

            throw new ArgumentException("File content cannot be empty", nameof(data));
        }
    }
}