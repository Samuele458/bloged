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
    public interface IGenericRepository<T> where T : class
    {

        /// <summary>
        ///  Get all entities
        /// </summary>
        /// <returns>All entities</returns>
        Task<IEnumerable<T>> All();

        /// <summary>
        ///  Get entity by Id
        /// </summary>
        /// <param name="Id">Entity Id</param>
        /// <returns>Entity object</returns>
        Task<T> GetById(Guid Id);

        /// <summary>
        ///  Add new entity
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> Add(T entity);

        /// <summary>
        ///  Delete a specified entity
        /// </summary>
        /// <param name="id">Entity Id</param>
        /// <param name="userId">User Id</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> Delete(Guid id, string userId);

        /// <summary>
        ///  Modify or add an entity (if not exists)
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> Upsert(T entity);

    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        protected DataContext dataContext;

        internal DbSet<T> dbSet;

        protected readonly ILogger logger;

        public GenericRepository(
                DataContext context,
                ILogger logger
            )
        {
            this.dataContext = context;
            this.dbSet = context.Set<T>();
            this.logger = logger;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> Add(T entity)
        {
            await dataContext.AddAsync(entity);
            return true;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<T>> All()
        {
            return await dbSet.ToListAsync();
        }

        /// <inheritdoc/>
        public Task<bool> Delete(Guid id, string userId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual Task<T> GetById(Guid Id)
        {
            throw new NotImplementedException();
            //return await dbSet.FindAsync(Id)
        }

        /// <inheritdoc/>
        public Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
