using FileSystem.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystem.Application
{
    public class TransactionalPipelineBehaviour<TRequest, TResponse> : 
        IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionalPipelineBehaviour(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            CancellationToken cancellationToken, 
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            await _unitOfWork.SaveChangesAsync();
            return response;
        }
    }
}
