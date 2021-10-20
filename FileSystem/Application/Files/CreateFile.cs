using FileSystem.Domain.Directories;
using FileSystem.Domain.Files;
using FileSystem.Infrastructure.Directories;
using FileSystem.Infrastructure.Files;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystem.Application.Files
{
    public static class CreateFile
    {
        public class Request : IRequest
        {
            public long ParentId { get; }
            public string Name { get; }
            public byte[] Content { get; }

            public Request(long parentId, string name, byte[] content)
            {
                ParentId = parentId;
                Name = name;
                Content = content;
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
                var parent = await _directoryRepository.GetById(new DirectoryId(request.ParentId));
                if (parent is null)
                {
                    throw new InvalidOperationException("Directory not found");
                }

                var fileName = FileName.Create(request.Name);
                var content = FileContent.Create(request.Content);
                var file = new File(parent.Id, fileName, content);
                _fileRepository.Add(file);
                return Unit.Value;
            }
        }
    }
}
