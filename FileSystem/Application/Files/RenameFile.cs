using FileSystem.Domain.Files;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Infrastructure.Files;

namespace FileSystem.Application.Files
{
    public static class RenameFile
    {
        public class Request : IRequest
        {
            public long FileId { get; }
            public string Name { get; }

            public Request(long fileId, string name)
            {
                FileId = fileId;
                Name = name;
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IFileRepository _fileRepository;

            public Handler(IFileRepository fileRepository)
            {
                _fileRepository = fileRepository;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var id = new FileId(request.FileId);
                var file = await _fileRepository.GetById(id);

                if (file is null)
                {
                    throw new InvalidOperationException("File not found");
                }

                var fileName = FileName.Create(request.Name);
                file.Rename(fileName);
                _fileRepository.Update(file);
                return Unit.Value;
            }
        }
    }
}