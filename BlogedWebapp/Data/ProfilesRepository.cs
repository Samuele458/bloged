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

    public interface IProfilesRepository : IGenericRepository<Profile>
    {

        //sTask<Profile> GetUserByEmailAddress(string email);
    }

    public class ProfilesRepository : GenericRepository<Profile>, IProfilesRepository
    {

        public ProfilesRepository(
                DataContext context,
                ILogger logger
            ) : base(context, logger)
        {

        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<Profile>> All()
        {
            try
            {
                return await dbSet.Where(x => x.Status == 1)
                                .Include(u => u.Identity)
                                .AsNoTracking()
                                .ToListAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(ProfilesRepository));
                return new List<Profile>();
            }
        }

        /// <inheritdoc/>
        public override async Task<Profile> GetById(Guid Id)
        {
            try
            {
                return await dbSet
                                .FirstOrDefaultAsync(u => u.Id == Id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(ProfilesRepository));
                return new Profile();
            }
        }
    }

}
