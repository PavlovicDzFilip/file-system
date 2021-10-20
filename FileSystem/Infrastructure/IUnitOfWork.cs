using System.Threading.Tasks;

namespace FileSystem.Infrastructure
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}