using System.Linq;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Domain.Files;
using Microsoft.EntityFrameworkCore;

namespace FileSystem.Infrastructure.Files
{
    public class FileRepository : IFileRepository
    {
        private readonly Database _database;

        public FileRepository(Database database) 
            => _database = database;

        public Task<File> GetById(FileId fileId)
            => _database
                .Files
                .AsNoTracking()
                .FirstOrDefaultAsync(file => file.Id == fileId);

        public Task<File[]> GetInDirectory(DirectoryId directoryId)
            => _database
                .Files
                .AsNoTracking()
                .Where(file => file.ParentId == directoryId)
                .ToArrayAsync();

        public void Add(File file)
            => _database.Files.Add(file);

        public void AddRange(File[] files)
            => _database.Files.AddRange(files);

        public void Update(File file)
            => _database.Entry(file).State = EntityState.Modified;

        public void Remove(File file)
            => _database.Files.Remove(file);

        public Task<File[]> GetInDirectories(DirectoryId[] directoryIds)
            => _database
                .Files
                .Where(file => directoryIds.Contains(file.ParentId))
                .AsNoTracking()
                .ToArrayAsync();
    }
}