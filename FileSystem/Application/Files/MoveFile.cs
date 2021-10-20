using FileSystem.Domain.Directories;
using FileSystem.Domain.Files;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Infrastructure.Directories;
using FileSystem.Infrastructure.Files;

namespace FileSystem.Application.Files
{
    public static class MoveFile
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
            private readonly IFileRepository _fileRepository;
            private readonly IDirectoryRepository _directoryRepository;

            public Handler(
                IFileRepository fileRepository,
                IDirectoryRepository directoryRepository)
            {
                _fileRepository = fileRepository;
                _directoryRepository = directoryRepository;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var file = await _fileRepository.GetById(new FileId(request.Id));
                var destinationDirectory = await _directoryRepository.GetById(new DirectoryId(request.DestinationParentId));

                if (file is null)
                {
                    throw new InvalidOperationException("File not found");
                }

                if (destinationDirectory is null)
                {
                    throw new InvalidOperationException("Directory not found");
                }

                file.SetParent(destinationDirectory);
                _fileRepository.Update(file);
                return Unit.Value;
            }
        }
    }
}