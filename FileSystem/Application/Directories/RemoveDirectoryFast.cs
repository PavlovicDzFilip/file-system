using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Infrastructure.Directories;
using MediatR;

namespace FileSystem.Application.Directories
{
    public static class RemoveDirectoryFast
    {
        public class Request : IRequest
        {
            public long Id { get; }

            public Request(long id)
            {
                Id = id;
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IDirectoryRepository _directoryRepository;

            public Handler(
                IDirectoryRepository directoryRepository)
            {
                _directoryRepository = directoryRepository;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                await _directoryRepository.RemoveFast(new DirectoryId(request.Id));
                return Unit.Value;
            }
        }
    }
}