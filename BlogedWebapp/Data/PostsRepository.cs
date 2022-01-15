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
    public interface IPostsRepository : IBlogDependentRepository<Post>
    {
        Task<Post> GetByUrlName(
            string blogId,
            string urlName,
            ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal
        );

        Task<PostsTag> AddTagToPost(Post post, Tag tag);

        bool RemoveTagFromPost(PostsTag postTag);

    }

    public class PostsRepository : BlogDependentRepository<Post>, IPostsRepository
    {
        public PostsRepository(
                DataContext context,
                IUnitOfWork unitOfWork,
                ILogger logger
            ) : base(context, unitOfWork, logger)
        {


        }

        public override async Task<bool> Add(string blogId, Post entityObject)
        {
            if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
            {
                await dbSet.AddAsync(entityObject);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Post>> All(string blogId, ProjectionBehaviour projectionBehaviour)
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {
                    return await dbSet.Where(x => x.Status == 1)
                                .AsNoTracking()
                                .ToListAsync();
                }
                else
                {
                    return new List<Post>();
                }
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
                return await ProjectionHelper<Post>
                                .BuildProjection(dbSet, projectionBehaviour)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Id == Id.ToString());
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetById\" method has generated an error.", typeof(PostsRepository));
                return null;
            }
        }

        public override async Task<Post> Update(string blogId, Post entityObject)
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {
                    Post post = await ProjectionHelper<Post>
                                .BuildProjection(dbSet, ProjectionBehaviour.Full)
                                .FirstOrDefaultAsync(u => u.Id == entityObject.Id);

                    if (entityObject != null)
                    {
                        EntityUpdater.Update(post, entityObject);
                    }

                    return post;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"Update\" method has generated an error.", typeof(PostsRepository));
                return null;
            }
        }

        public async Task<Post> GetByUrlName(
            string blogId,
            string urlName,
            ProjectionBehaviour projectionBehaviour
        )
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {
                    return await ProjectionHelper<Post>.BuildProjection(dbSet, projectionBehaviour)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.UrlName.Equals(urlName));
                }
                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetByUrlName\" method has generated an error.", typeof(PostsRepository));
                return null;
            }
        }


        public async Task<PostsTag> AddTagToPost(Post post, Tag tag)
        {
            try
            {
                PostsTag postTag = new PostsTag()
                {
                    PostId = post.Id,
                    TagId = tag.Id
                };

                await dataContext.Set<PostsTag>().AddAsync(postTag);

                return postTag;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetByUrlName\" method has generated an error.", typeof(PostsRepository));
                return null;
            }
        }

        public bool RemoveTagFromPost(PostsTag postTag)
        {
            try
            {
                dataContext.Set<PostsTag>().Remove(postTag);

                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetByUrlName\" method has generated an error.", typeof(PostsRepository));
                return false;
            }
        }
    }
}
