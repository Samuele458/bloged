using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public UsersController( IUnitOfWork unitOfWork )
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetUsers()
        {
            var users = await unitOfWork.Users.All();
            return Ok(users);
        }


        [HttpPost]
        public async Task<IActionResult> AddUser( CreateUserRequestDto request )
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


            return CreatedAtRoute("GetUser", user.Id, request);
        }



    }
}
