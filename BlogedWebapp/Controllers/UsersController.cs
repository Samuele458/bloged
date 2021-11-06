﻿using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlogedWebapp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository UserRepository;


        public UsersController(IUserRepository userRepository)
        {
            this.UserRepository = userRepository;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {

            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {

            User user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            UserRepository.Create(user);

            return Ok();

        }

    }
}