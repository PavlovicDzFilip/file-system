using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Infrastructure.Directories;
using MediatR;

namespace FileSystem.Application.Directories
{
    public static class CreateDirectoryNaive
    {
        public class Request : IRequest
        {
            public string Path { get; }

            public Request(string path)
            {
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
                var path = Path.Parse(request.Path);
                var parentId = DirectoryId.NoParent;
                foreach (var directoryName in path)
                {
                    var parentDir = await _directoryRepository.GetByNameForParent(parentId, directoryName);
                    if (parentDir is null)
                    {
                        parentDir = new Directory(parentId, directoryName);
                        _directoryRepository.Add(parentDir);
                    }

                    parentId = parentDir.Id;
                }
                
                return Unit.Value;
            }
        }
    }
}