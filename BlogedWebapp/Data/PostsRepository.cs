using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{
    public interface IPostsRepository : IGenericRepository<Post>
    {
        Task<Post> GetByUrlName(string urlName);
    }

    public class PostsRepository : GenericRepository<Post>, IPostsRepository
    {
        public PostsRepository(
                DataContext context,
                IUnitOfWork unitOfWork,
                ILogger logger
            ) : base(context, unitOfWork, logger)
        {


        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Post>> All(ProjectionBehaviour projectionBehaviour)
        {
            try
            {
                return await dbSet.Where(x => x.Status == 1)
                                .AsNoTracking()
                                .ToListAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(PostsRepository));
                return new List<Post>();
            }
        }

        /// <inheritdoc/>
        public override async Task<Post> GetById(Guid Id, ProjectionBehaviour projectionBehaviour)
        {
            try
            {
                return await dbSet
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Id == Id.ToString());
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetById\" method has generated an error.", typeof(PostsRepository));
                return new Post();
            }
        }

        public override async Task<bool> Update(Post post)
        {
            try
            {
                await dbSet
                        .FirstOrDefaultAsync(u => u.Id == post.Id);

                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetById\" method has generated an error.", typeof(PostsRepository));
                return false;
            }
        }

        public async Task<Post> GetByUrlName(string urlName)
        {
            try
            {
                return await dbSet
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.UrlName.Equals(urlName));
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetByUrlName\" method has generated an error.", typeof(PostsRepository));
                return new Post();
            }
        }
    }
}
