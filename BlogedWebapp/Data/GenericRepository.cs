using BlogedWebapp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{
    /// <summary>
    ///  Generic repository interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IGenericRepository<T> : IBaseRepository<T> where T : class
    {

        /// <summary>
        ///  Get all entities
        /// </summary>
        /// <returns>All entities</returns>
        Task<IEnumerable<T>> All(ProjectionBehaviour projectionBehaviour = ProjectionBehaviour.Normal);

        /// <summary>
        ///  Add new entity
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> Add(T entity);

        /// <summary>
        ///  Update entity
        /// </summary>
        /// <param name="entity">Entity object to update</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> Update(T entity);

        /// <summary>
        ///  Delete a specified entity
        /// </summary>
        /// <param name="entity">Entity Id</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> Delete(T entity);

        /// <summary>
        ///  Modify or add an entity (if not exists)
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> Upsert(T entity);

    }

    public class GenericRepository<T> : BaseRepository<T>, IGenericRepository<T> where T : class
    {


        public GenericRepository(
            DataContext context,
            IUnitOfWork unitOfWork,
            ILogger logger
        ) : base(context, unitOfWork, logger)
        { }

        /// <inheritdoc/>
        public virtual async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual Task<bool> Update(T entity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<T>> All(ProjectionBehaviour projectionBehaviour)
        {
            return await dbSet.ToListAsync();
        }

        /// <inheritdoc/>
        public virtual Task<bool> Delete(T entity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
