using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Domain.Files;

namespace FileSystem.Infrastructure.Files
{
    public interface IFileRepository
    {
        Task<File> GetById(FileId fileId);
        Task<File[]> GetInDirectory(DirectoryId directoryId);
        void Add(File file);
        void Update(File file);
        void Remove(File file);
        Task<File[]> GetInDirectories(DirectoryId[] directoryIds);
    }
}