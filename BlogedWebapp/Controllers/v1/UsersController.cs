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
    ///  Users controller
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : BaseController
    {
        private readonly IAuthorizationService authorizationService;

        public UsersController(IUnitOfWork unitOfWork, IAuthorizationService authorizationService) : base(unitOfWork)
        {
            this.authorizationService = authorizationService;
        }

        /// <summary>
        ///  Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetUsers()
        {
            var users = await unitOfWork.Users.All();
            System.Diagnostics.Debug.WriteLine("Identity: ", users);
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
            User user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Status = 1,
                Password = request.Password
            };

            await unitOfWork.Users.Add(user);
            await unitOfWork.CompleteAsync();


            return CreatedAtRoute("GetUser", new { id = user.Id }, request);
        }

        /// <summary>
        ///  Get a specified user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Selected user</returns>
        [HttpGet]
        [Route("GetUser", Name = "GetUser")]
        //[Authorize(Roles = "Admin")]
        //[Authorize( Policy = "TestPolicy")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            User user = await unitOfWork.Users.GetById(id);
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
