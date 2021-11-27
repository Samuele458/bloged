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

    public interface IBlogsRepository : IGenericRepository<Blog>
    {
        Task<UsersBlog> SetBlogOwner(Blog blog, AppUser user);

        Task<Blog> GetByUrlName(string urlName);
    }

    public class BlogsRepository : GenericRepository<Blog>, IBlogsRepository
    {
        public BlogsRepository(
                DataContext context,
                ILogger logger
            ) : base(context, logger)
        {


        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Blog>> All()
        {
            try
            {
                return await dbSet.Where(x => x.Status == 1)
                                .Include(b => b.UsersBlog)
                                .AsNoTracking()
                                .ToListAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(BlogsRepository));
                return new List<Blog>();
            }
        }

        /// <inheritdoc/>
        public override async Task<Blog> GetById(Guid Id)
        {
            try
            {
                return await dbSet
                                .Include(u => u.UsersBlog)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Id == Id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetById\" method has generated an error.", typeof(BlogsRepository));
                return new Blog();
            }
        }

        public async Task<Blog> GetByUrlName( string urlName )
        {
            try
            {
                return await dbSet
                                .Include(u => u.UsersBlog)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.UrlName.Equals(urlName));
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"GetByUrlName\" method has generated an error.", typeof(BlogsRepository));
                return new Blog();
            }
        }

        public async Task<UsersBlog> SetBlogOwner(Blog blog, AppUser user)
        {
            try
            {
                UsersBlog usersBlog = new UsersBlog()
                {
                    Blog = blog,
                    User = user,
                    Role = "Owner"
                };

                await this.dataContext.Set<UsersBlog>().AddAsync(usersBlog);

                return usersBlog;
            } catch(Exception e)
            {
                logger.LogError(e, "{Repo} \"SetBlogOwner\" method has generated an error.", typeof(BlogsRepository));
                return new UsersBlog();
            }

        }
    }
}
