using FileSystem.Domain.Files;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystem.Infrastructure.Files;

namespace FileSystem.Application.Files
{
    public static class RemoveFile
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
            private readonly IFileRepository _fileRepository;

            public Handler(IFileRepository fileRepository)
            {
                _fileRepository = fileRepository;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var file = await _fileRepository.GetById(new FileId(request.Id));
                if (file is null)
                {
                    throw new InvalidOperationException("File not found");
                }

                _fileRepository.Remove(file);
                return Unit.Value;
            }
        }
    }
}