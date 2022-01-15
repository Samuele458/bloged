using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{

    public interface ITagsRepository : IBlogDependentRepository<Tag>
    {
        Task<Tag> GetByUrlName(
            string blogId,
            string tagUrlName,
            ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal
        );
    }

    public class TagsRepository : BlogDependentRepository<Tag>, ITagsRepository
    {

        public TagsRepository(
            DataContext context,
            IUnitOfWork unitOfWork,
            ILogger logger
        ) : base(context, unitOfWork, logger)
        { }

        public override async Task<bool> Add(string blogId, Tag entityObject)
        {
            if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
            {
                await dbSet.AddAsync(entityObject);
                return true;
            }

            return false;
        }

        public override async Task<Tag> GetById(
            Guid Id,
            ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal
        )
        {
            try
            {

                return await ProjectionHelper<Tag>
                                .BuildProjection(dbSet, projectionBehaviour)
                                .FirstOrDefaultAsync(u => u.Id == Id.ToString());

            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetById\" method has generated an error.", typeof(TagsRepository));
                return null;
            }
        }

        public async Task<Tag> GetByUrlName(
            string blogId,
            string tagUrlName,
            ProjectionBehaviour projectionBehaviour
        )
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {

                    return await ProjectionHelper<Tag>
                                    .BuildProjection(dbSet, projectionBehaviour)
                                    .FirstOrDefaultAsync(u => u.UrlName == tagUrlName);
                }

                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetByUrlName\" method has generated an error.", typeof(TagsRepository));
                return null;
            }
        }

        public override async Task<Tag> Update(string blogId, Tag entityObject)
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {
                    Tag tag = await dbSet.FirstOrDefaultAsync(u => u.Id == entityObject.Id);

                    if (entityObject != null)
                    {
                        EntityUpdater.Update(tag, entityObject);
                    }

                    return tag;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"Update\" method has generated an error.", typeof(TagsRepository));
                return null;
            }
        }

        public override async Task<bool> Delete(string blogId, Tag entityObject)
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {
                    var tagObj = await dbSet
                        .FirstOrDefaultAsync(u => u.Id == entityObject.Id);

                    dbSet.Remove(tagObj);

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"Delete\" method has generated an error.", typeof(TagsRepository));
                return false;
            }
        }
    }
}
