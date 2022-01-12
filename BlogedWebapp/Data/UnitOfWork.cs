using BlogedWebapp.Helpers;
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

        IBlogsRepository Blogs { get; }

        IPostsRepository Posts { get; }

        ICategoriesRepository Categories { get; }

        ITagsRepository Tags { get; }

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

        public IBlogsRepository Blogs { get; private set; }

        public IPostsRepository Posts { get; private set; }

        public ICategoriesRepository Categories { get; private set; }

        public ITagsRepository Tags { get; private set; }

        public UnitOfWork(
            DataContext context,
            ILoggerFactory loggerFactory)
        {
            this.context = context;
            this.logger = loggerFactory.CreateLogger("db_logs");

            Profiles = new ProfilesRepository(context, this, logger);
            RefreshTokens = new RefreshTokensRepository(context, this, logger);
            Blogs = new BlogsRepository(context, this, logger);
            Posts = new PostsRepository(context, this, logger);
            Categories = new CategoriesRepository(context, this, logger);
            Tags = new TagsRepository(context, this, logger);
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
