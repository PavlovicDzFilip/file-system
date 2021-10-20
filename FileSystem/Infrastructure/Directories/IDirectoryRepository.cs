using System.Threading.Tasks;
using FileSystem.Domain.Directories;

namespace FileSystem.Infrastructure.Directories
{
    public interface IDirectoryRepository
    {
        Task<Directory> GetById(DirectoryId directoryId);
        Task<Directory[]> GetByIdWithAllChildren(DirectoryId directoryId);
        Task<Directory> GetByNameForParent(DirectoryId parentId, DirectoryName directoryName);
        void Add(Directory directory);
        void AddRange(Directory[] directories);
        Task<Directory[]> GetChildren(DirectoryId parentId);
        Task<DirectoryPath> GetDirectoriesInPath(Path path);
        Task<Directory> GetLastDirectoryInPath(Path path);
        void Update(Directory directory);
        void Remove(Directory directory);
        Task RemoveFast(DirectoryId directoryId);
    }
}