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
    public interface IPostsRepository
    {

    }

    public class PostsRepository : GenericRepository<Post>, IPostsRepository
    {
        public PostsRepository(
                DataContext context,
                ILogger logger
            ) : base(context, logger)
        {


        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Post>> All()
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
        public override async Task<Post> GetById(Guid Id)
        {
            try
            {
                return await dbSet
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Id == Id);
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
    }
}
