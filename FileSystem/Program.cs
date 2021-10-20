using FileSystem.Application;
using FileSystem.Application.Directories;
using FileSystem.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using FileSystem.Infrastructure.Directories;
using FileSystem.Infrastructure.Files;
using Newtonsoft.Json;

namespace FileSystem
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = CreateProvider();
            var command = new ListDirectories.Request("projects/file-system/data");
            var response = await Send(command, provider);
            Console.WriteLine(JsonConvert.SerializeObject(response));
        }

        private static async Task<T> Send<T>(IRequest<T> request, IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var task = sender.Send(request);
            return await task;
        }

        private static IServiceProvider CreateProvider()
        {
            var services = new ServiceCollection();
            services.AddDbContext<Database>(
                options => options
                    .UseSqlServer(
                        "User ID=sa;Password=****;Initial Catalog=FileSystem;Server=localhost"));

            services.AddMediatR(typeof(Program).Assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionalPipelineBehaviour<,>));
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Database>());
            services.AddScoped<IDirectoryRepository, DirectoryRepository>();
            services.AddScoped<IFileRepository, FileRepository>();

            return services.BuildServiceProvider();

        }
    }
}
