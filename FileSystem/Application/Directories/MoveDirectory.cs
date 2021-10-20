using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Infrastructure.Directories;
using MediatR;

namespace FileSystem.Application.Directories
{
    public static class MoveDirectory
    {
        public class Request : IRequest
        {
            public long Id { get; }
            public long DestinationParentId { get; }

            public Request(long id, long destinationParentId)
            {
                Id = id;
                DestinationParentId = destinationParentId;
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
                var directory = await _directoryRepository.GetById(new DirectoryId(request.Id));
                var destinationDirectory = await _directoryRepository.GetById(new DirectoryId(request.DestinationParentId));

                if (directory is null || destinationDirectory is null)
                {
                    throw new InvalidOperationException("Directory not found");
                }

                directory.SetParent(destinationDirectory);
                _directoryRepository.Update(directory);
                return Unit.Value;
            }
        }
    }
}