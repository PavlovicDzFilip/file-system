using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.DTOs;
using FileSystem.Infrastructure.Directories;
using FileSystem.Infrastructure.Files;
using MediatR;

namespace FileSystem.Application.Files
{
    public static class ListFiles
    {
        public class Request : IRequest<FileDto[]>
        {
            public string Path { get; }

            public Request(string path)
            {
                Path = path;
            }
        }

        public class Handler : IRequestHandler<Request, FileDto[]>
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

            public async Task<FileDto[]> Handle(Request request, CancellationToken cancellationToken)
            {
                var path = Path.Parse(request.Path);
                var directory = await _directoryRepository.GetLastDirectoryInPath(path);
                if (directory is null)
                {
                    return Array.Empty<FileDto>();
                }

                var children = await _fileRepository.GetInDirectory(directory.Id);
                return children
                    .Select(child => new FileDto(child.Id.Value, child.Name.Value, child.Content.Content))
                    .ToArray();
            }
        }
    }
}