using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.Infrastructure.Directories;
using MediatR;

namespace FileSystem.Application.Directories
{
    public static class RenameDirectory
    {
        public class Request : IRequest
        {
            public long DirectoryId { get; }
            public string Name { get; }

            public Request(long directoryId, string name)
            {
                DirectoryId = directoryId;
                Name = name;
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
                var id = new DirectoryId(request.DirectoryId);
                var directory = await _directoryRepository.GetById(id);

                if (directory is null)
                {
                    throw new InvalidOperationException("Directory not found");
                }

                var directoryName = DirectoryName.Create(request.Name);
                directory.Rename(directoryName);
                _directoryRepository.Update(directory);
                return Unit.Value;
            }
        }
    }
}