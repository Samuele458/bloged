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

    public interface IUsersRepository : IGenericRepository<User>
    {

        //sTask<User> GetUserByEmailAddress(string email);
    }

    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {

        public UsersRepository(
                DataContext context,
                ILogger logger
            ) : base(context, logger)
        {

        }

        public override async Task<IEnumerable<User>> All()
        {
            try
            {
                return await dbSet.Where(x => x.Status == 1)
                                .AsNoTracking()
                                .ToListAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(UsersRepository));
                return new List<User>();
            }
        }
    }

}
