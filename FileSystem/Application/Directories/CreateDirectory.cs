using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Infrastructure.Directories;
using MediatR;

namespace FileSystem.Application.Directories
{
    public static class CreateDirectory
    {
        public class Request : IRequest
        {
            public long ParentId { get; }
            public string Path { get; }

            public Request(long parentId, string path)
            {
                ParentId = parentId;
                Path = path;
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IDirectoryRepository _directoryRepository;

            public Handler(IDirectoryRepository directoryRepository)
            {
                _directoryRepository = directoryRepository;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var parent = await _directoryRepository.GetById(new DirectoryId(request.ParentId));
                var path = Path.Parse(request.Path);
                var directories = parent.CreateChildren(path);
                _directoryRepository.AddRange(directories.ToArray());
                return Unit.Value;
            }
        }
    }
}
