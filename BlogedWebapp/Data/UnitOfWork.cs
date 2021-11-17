using BlogedWebapp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{

    /// <summary>
    ///  UnitOfWork Interface
    /// </summary>
    public interface IUnitOfWork
    {
        IProfilesRepository Profiles { get; }

        IRefreshTokensRepository RefreshTokens { get; }

        /// <summary>
        ///  Save db changes on context asynchronously
        /// </summary>
        /// <returns>Completed task</returns>
        Task CompleteAsync();
    }

    /// <summary>
    ///  UnitOfWork class
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {

        private readonly DataContext context;

        private readonly ILogger logger;

        public IProfilesRepository Profiles { get; private set; }

        public IRefreshTokensRepository RefreshTokens { get; private set; }

        

        public UnitOfWork(
            DataContext context, 
            ILoggerFactory loggerFactory)
        {
            this.context = context;
            this.logger = loggerFactory.CreateLogger("db_logs");

            Profiles = new ProfilesRepository(context, logger);
            RefreshTokens = new RefreshTokensRepository(context, logger);
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            await context.SaveChangesAsync();
        }

        /// <summary>
        ///  Dispose resources
        /// </summary>
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
