using BlogedWebapp.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{

    public interface IBlogDependentRepository<T> : IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> All(string blogId, ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal);

        Task<T> GetById(
            string blogId,
            string id,
            ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal
        );


        Task<bool> Add(string blogId, T entityObject);

        Task<bool> Update(string blogId, T entityObject);

        Task<bool> Delete(string blogId, T entityObject);
    }

    public class BlogDependentRepository<T> : BaseRepository<T>, IBlogDependentRepository<T> where T : class
    {
        public BlogDependentRepository(
            DataContext context,
            IUnitOfWork unitOfWork,
            ILogger logger
        ) : base(context, unitOfWork, logger)
        { }

        public virtual async Task<bool> Add(string blogId, T entityObject)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> All(string blogId, ProjectionBehaviour projectionBehaviour)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<bool> Delete(string blogId, T entityObject)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<T> GetById(string blogId, string id, ProjectionBehaviour projectionBehaviour)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<bool> Update(string blogId, T entityObject)
        {
            throw new System.NotImplementedException();
        }
    }

}
