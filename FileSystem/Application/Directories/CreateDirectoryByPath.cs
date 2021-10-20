using FileSystem.Domain.Directories;
using FileSystem.Infrastructure.Directories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystem.Application.Directories
{
    public static class CreateDirectoryByPath
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
                var directoryPath = await _directoryRepository.GetDirectoriesInPath(path);

                var missingDirectories = directoryPath.Current.CreateChildren(path, directoryPath.Next);
                _directoryRepository.AddRange(missingDirectories.ToArray());
                return Unit.Value;
            }
        }
    }
}