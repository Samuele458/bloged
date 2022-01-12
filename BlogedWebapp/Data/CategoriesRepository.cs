using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{

    public interface ICategoriesRepository : IBlogDependentRepository<Category>
    {
        Task<Category> GetByUrlName(string blogId, string categoryUrlName, ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal);
    }

    public class CategoriesRepository : BlogDependentRepository<Category>, ICategoriesRepository
    {
        public CategoriesRepository(
            DataContext context,
            IUnitOfWork unitOfWork,
            ILogger logger
        ) : base(context, unitOfWork, logger)
        { }

        public override async Task<bool> Add(string blogId, Category entityObject)
        {
            if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
            {
                await dbSet.AddAsync(entityObject);
                return true;
            }

            return false;
        }


        public override async Task<Category> GetById(
            Guid Id,
            ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal
        )
        {
            try
            {

                return await ProjectionHelper<Category>
                                .BuildProjection(dbSet, projectionBehaviour)
                                .FirstOrDefaultAsync(u => u.Id == Id.ToString());

            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetById\" method has generated an error.", typeof(CategoriesRepository));
                return null;
            }
        }

        public async Task<Category> GetByUrlName(
            string blogId,
            string categoryUrlName,
            ProjectionBehaviour projectionBehaviour
        )
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {

                    return await ProjectionHelper<Category>
                                    .BuildProjection(dbSet, projectionBehaviour)
                                    .FirstOrDefaultAsync(u => u.UrlName == categoryUrlName);
                }

                return null;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetByUrlName\" method has generated an error.", typeof(CategoriesRepository));
                return null;
            }
        }

        public override async Task<bool> Update(string blogId, Category entityObject)
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {
                    Category category = await dbSet.FirstOrDefaultAsync(u => u.Id == entityObject.Id);

                    EntityUpdater.Update(category, entityObject);

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"Update\" method has generated an error.", typeof(BlogsRepository));
                return false;
            }
        }

        public override async Task<bool> Delete(string blogId, Category entityObject)
        {
            try
            {
                if (this.unitOfWork.Blogs.GetById(Guid.Parse(blogId), ProjectionBehaviour.Preview) != null)
                {
                    var categoryObj = await dbSet
                        .FirstOrDefaultAsync(u => u.Id == entityObject.Id);

                    dbSet.Remove(categoryObj);

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"Delete\" method has generated an error.", typeof(BlogsRepository));
                return false;
            }
        }
    }
}
