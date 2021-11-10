using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers.v1
{
    /// <summary>
    ///  Authentication handling
    /// </summary>
    public class AccountsController : BaseController
    {

        private readonly UserManager<IdentityUser> userManager;

        private readonly AppSettings appSettings;
        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<AppSettings> optionsMonitor) : base(unitOfWork)
        {
            this.userManager = userManager;
            this.appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        ///  User registration
        /// </summary>
        /// <param name="registrationDto">Registration Data Transfer Object</param>
        /// <returns>User Registration response DTO</returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationDto)
        {
            if (ModelState.IsValid)
            {

                //checking if email was already taken
                var userExists = await userManager.FindByEmailAsync(registrationDto.Email);

                if (userExists != null)
                {
                    //email already taken
                    return BadRequest(new UserRegistrationResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Email Already in use"
                        },
                    });
                }

                //Adding new user
                var newUser = new IdentityUser()
                {
                    Email = registrationDto.Email,
                    UserName = registrationDto.Username,
                    EmailConfirmed = true
                };

                //creating new user
                var userCreated = await userManager.CreateAsync(newUser, registrationDto.Password);
                if (!userCreated.Succeeded)
                {
                    //errors on user creation
                    return BadRequest(new UserRegistrationResponseDto
                    {
                        Success = false,
                        Errors = userCreated.Errors.Select(x => x.Description).ToList()
                    });
                }

                //creting new user object
                User user = new User()
                {
                    Identity = newUser,
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Username = registrationDto.Username,
                    Email = registrationDto.Email,
                    Status = 1,
                    Password = registrationDto.Password,
                    UpdatedOn = DateTime.UtcNow
                };

                await unitOfWork.Users.Add(user);
                await unitOfWork.CompleteAsync();

                //generating new JWT
                var token = GenerateJwtToken(newUser);

                return Ok(new UserRegistrationResponseDto
                {
                    Success = true,
                    Token = token
                });
            }
            else
            {
                //invalid object
                return BadRequest(new UserRegistrationResponseDto
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    },
                });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
        {
            if (ModelState.IsValid)
            {
                //checking if user exists
                var userExists = await userManager.FindByEmailAsync(loginDto.Email);

                if (userExists == null)
                {
                    //User does not exist
                    return BadRequest(new UserLoginResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Invalid authentication request"
                        },
                    });
                }


                //checking password
                var isPasswordCorrect = await userManager.CheckPasswordAsync(userExists, loginDto.Password);

                if (!isPasswordCorrect)
                {
                    //invalid password
                    return BadRequest(new UserLoginResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Invalid authentication request"
                        },
                    });
                }

                //generating new JWT
                var token = GenerateJwtToken(userExists);

                return Ok(new UserLoginResponseDto
                {
                    Success = true,
                    Token = token
                });
            }
            else
            {
                //invalid object
                return BadRequest(new UserLoginResponseDto
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    },
                });
            }
        }

        /// <summary>
        ///  Generate new JWT
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>JWT string</returns>
        private string GenerateJwtToken(IdentityUser user)
        {
            //token handler
            var jwtHandler = new JwtSecurityTokenHandler();

            //getting security key
            var key = Encoding.ASCII.GetBytes(appSettings.JwtSecret);

            //creating token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //creating token based on token descripton
            var token = jwtHandler.CreateToken(tokenDescriptor);

            //converting token into a string
            var jwtToken = jwtHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
