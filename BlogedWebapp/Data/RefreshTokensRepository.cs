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
    public interface IRefreshTokensRepository : IGenericRepository<RefreshToken>
    {

    }

    public class RefreshTokensRepository : GenericRepository<RefreshToken>, IRefreshTokensRepository
    {

        public RefreshTokensRepository(
                DataContext context,
                ILogger logger
            ) : base(context, logger)
        {

        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<RefreshToken>> All()
        {
            try
            {
                return await dbSet.Where(x => x.Status == 1)
                                .AsNoTracking()
                                .ToListAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(RefreshTokensRepository));
                return new List<RefreshToken>();
            }
        }

        /// <inheritdoc/>
        public override async Task<RefreshToken> GetById(Guid Id)
        {
            try
            {
                return await dbSet
                                .FirstOrDefaultAsync(u => u.Id == Id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(RefreshTokensRepository));
                return new RefreshToken();
            }
        }
    }
}
