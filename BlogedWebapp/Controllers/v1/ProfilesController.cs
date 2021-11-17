using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await unitOfWork.Profiles.All();
            return Ok(users);
        }

        /// <summary>
        ///  Add a new user
        /// </summary>
        /// <param name="request">Request DTO</param>
        /// <returns>Created user info</returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(CreateUserRequestDto request)
        {
            Profile user = new Profile()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Status = 1,
                Password = request.Password
            };

            await unitOfWork.Profiles.Add(user);
            await unitOfWork.CompleteAsync();


            return CreatedAtRoute("GetUser", new { id = user.Id }, request);
        }

        /// <summary>
        ///  Get a specified user by id
        /// </summary>
        /// <param name="id">Profile id</param>
        /// <returns>Selected user</returns>
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            Profile user = await unitOfWork.Profiles.GetById(userId);
            //return Ok(user);
            
            var authorizationResult = await authorizationService
                                                .AuthorizeAsync(User, user, "TestPolicy");

            System.Diagnostics.Debug.WriteLine("Succeed: " + authorizationResult.ToString() );
            System.Diagnostics.Debug.WriteLine("Failure: " + authorizationResult.Failure );

            if (authorizationResult.Succeeded)
                return Ok(user);

            else return BadRequest(new
            {
                Error = "AAA"
            });
            
        }


    }
}
