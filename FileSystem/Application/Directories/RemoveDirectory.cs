using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain;
using FileSystem.Domain.Directories;
using FileSystem.Infrastructure.Directories;
using FileSystem.Infrastructure.Files;
using MediatR;

namespace FileSystem.Application.Directories
{
    public static class RemoveDirectory
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
            private readonly IFileRepository _fileRepository;

            public Handler(
                IDirectoryRepository directoryRepository,
                IFileRepository fileRepository)
            {
                _directoryRepository = directoryRepository;
                _fileRepository = fileRepository;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var directories = await GetSubdirectories(new DirectoryId(request.Id));
                var directoryIds = directories
                    .Select(dir => dir.Id)
                    .ToArray();

                var files = await _fileRepository.GetInDirectories(directoryIds);
                files.ForEach(_fileRepository.Remove);
                directories.Reverse().ForEach(_directoryRepository.Remove);
                return Unit.Value;
            }

            private async Task<Directory[]> GetSubdirectories(DirectoryId directoryId)
            {
                var directories = await _directoryRepository.GetByIdWithAllChildren(directoryId);
                if (directories.IsEmpty())
                {
                    throw new InvalidOperationException("Directory not found");
                }

                return directories;
            }
        }
    }
}