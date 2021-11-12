﻿using BlogedWebapp.Entities;
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
        /// <summary>
        ///  Get refresh token object by its string
        /// </summary>
        /// <param name="refreshToken">Refresh token string</param>
        /// <returns>Refresh token object (null if not found)</returns>
        Task<RefreshToken> GetByRefreshToken(string refreshToken);


        /// <summary>
        ///  Mark a specified refresh token as used
        /// </summary>
        /// <param name="refreshToken">Refresh token to be marked as used</param>
        /// <returns>True if success, false otherwise</returns>
        Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken);


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

        /// <inheritdoc/>
        public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
        {
            try
            {
                return await dbSet.Where(x => x.Token.ToLower() == refreshToken.ToLower() )
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(RefreshTokensRepository));
                return new RefreshToken();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken)
        {
            try
            {
                var token = await dbSet.Where(x => x.Token.ToLower() == refreshToken.Token.ToLower())
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
                
                if (token == null) return false;

                token.IsUsed = refreshToken.IsUsed;
                return true;

            }
            catch (Exception e)
            {
                logger.LogError(e, "{Repo} \"All\" method has generated an error.", typeof(RefreshTokensRepository));
                return false;
            }
        }
    }
}
