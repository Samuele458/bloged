using BlogedWebapp.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{

    public interface IUnitOfWork
    {
        IUsersRepository Users { get; }

        Task CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork, IDisposable
    {

        private readonly DataContext context;

        private readonly ILogger logger;

        public IUsersRepository Users { get; private set; }

        public UnitOfWork(DataContext context, ILoggerFactory loggerFactory)
        {
            this.context = context;
            this.logger = loggerFactory.CreateLogger("db_logs");

            Users = new UsersRepository(context, logger);
        }

        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
