using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers.v1
{

    /// <summary>
    ///  Profiles controller
    /// </summary>
    [Route("v{version:apiVersion}/profiles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProfilesController : BaseController
    {
        private readonly IAuthorizationService authorizationService;

        public ProfilesController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService) : base(unitOfWork)
        {
            this.authorizationService = authorizationService;
        }

        /// <summary>
        ///  Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [Authorize(Policy = "AdminOrSuperadmin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await unitOfWork.Profiles.All();
            return Ok(users);
        }

        /// <summary>
        ///  Get a specified user by id
        /// </summary>
        /// <param name="id">ProfileData id</param>
        /// <returns>Selected user</returns>
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            ProfileData user = await unitOfWork.Profiles.GetById(userId);
            
            var authorizationResult = await authorizationService
                                                .AuthorizeAsync(User, user, "AllowedToUse");

            if (authorizationResult.Succeeded)
            {
                return Ok(user);
            }
            else return Unauthorized();
            
        }


    }
}
