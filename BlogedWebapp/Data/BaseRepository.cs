using BlogedWebapp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{

    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        ///  Get entity by Id
        /// </summary>
        /// <param name="Id">Entity Id</param>
        /// <returns>Entity object</returns>
        Task<T> GetById(Guid Id, ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal);


    }

    public class BaseRepository<T> where T : class
    {
        protected DataContext dataContext;

        protected DbSet<T> dbSet;

        protected readonly ILogger logger;

        protected readonly IUnitOfWork unitOfWork;

        public BaseRepository(
            DataContext context,
            IUnitOfWork unitOfWork,
            ILogger logger
        )
        {
            this.dataContext = context;
            this.dbSet = context.Set<T>();
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public virtual Task<T> GetById(Guid Id, ProjectionBehaviour projectionBehaviour)
        {
            throw new NotImplementedException();
        }
    }
}
