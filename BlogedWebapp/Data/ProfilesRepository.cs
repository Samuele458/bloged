using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogedWebapp.Data
{

    public interface IProfilesRepository : IGenericRepository<ProfileData>
    {

        //sTask<ProfileData> GetUserByEmailAddress(string email);
    }

    public class ProfilesRepository : GenericRepository<ProfileData>, IProfilesRepository
    {

        public ProfilesRepository(
                DataContext context,
                ILogger logger
            ) : base(context, logger)
        {

        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<ProfileData>> All(ProjectionBehaviour projectionBehaviour)
        {
            try
            {

                return await ProjectionHelper<ProfileData>
                                .BuildProjection(dbSet, ProjectionBehaviour.Normal)
                                .AsNoTracking()
                                .ToListAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(ProfilesRepository));
                return new List<ProfileData>();
            }
        }

        /// <inheritdoc/>
        public override async Task<ProfileData> GetById(Guid Id, ProjectionBehaviour projectionBehaviour)
        {
            try
            {
                return await dbSet
                                .Include(u => u.Owner)
                                .FirstOrDefaultAsync(u => u.Id == Id.ToString());
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(ProfilesRepository));
                return new ProfileData();
            }
        }
    }

}
