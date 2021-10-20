using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Domain.Directories;
using FileSystem.DTOs;
using FileSystem.Infrastructure.Directories;
using MediatR;

namespace FileSystem.Application.Directories
{
    public static class ListDirectories
    {
        public class Request : IRequest<DirectoryDto[]>
        {
            public string Path { get; }

            public Request(string path)
            {
                Path = path;
            }
        }

        public class Handler : IRequestHandler<Request, DirectoryDto[]>
        {
            private readonly IDirectoryRepository _directoryRepository;

            public Handler(IDirectoryRepository directoryRepository)
            {
                _directoryRepository = directoryRepository;
            }

            public async Task<DirectoryDto[]> Handle(Request request, CancellationToken cancellationToken)
            {
                var path = Path.Parse(request.Path);
                var directory = await _directoryRepository.GetLastDirectoryInPath(path);
                if (directory is null)
                {
                    return Array.Empty<DirectoryDto>();
                }

                var children = await _directoryRepository.GetChildren(directory.Id);
                return children
                    .Select(child => new DirectoryDto(child.Id.Value, child.Name.Value))
                    .ToArray();
            }
        }
    }
}