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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class UsersController : BaseController
    {

        public UsersController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

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
        public async Task<IActionResult> GetUser(Guid id)
        {
            
            return Ok(await unitOfWork.Users.GetById(id));
        }


    }
}
