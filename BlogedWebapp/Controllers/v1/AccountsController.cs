using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using BlogedWebapp.Models.Dtos.Generic;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers.v1
{
    /// <summary>
    ///  Accounts handling
    /// </summary>
    [Route("v{version:apiVersion}/accounts")]
    public class AccountsController : BaseController
    {

        private readonly UserManager<IdentityUser> userManager;

        private readonly RoleManager<IdentityRole> roleManager;

        private readonly TokenValidationParameters tokenValidationParameters;


        private readonly AppSettings appSettings;
        public AccountsController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TokenValidationParameters tokenValidationParameters,
            IOptionsMonitor<AppSettings> optionsMonitor) : base(unitOfWork)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.appSettings = optionsMonitor.CurrentValue;
            this.tokenValidationParameters = tokenValidationParameters;
        }

        /// <summary>
        ///  User registration
        /// </summary>
        /// <param name="registrationDto">Registration Data Transfer Object</param>
        /// <returns>User Registration response DTO</returns>
        [HttpPost]
        [Route("register")]
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
                Profile user = new Profile()
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

                await unitOfWork.Profiles.Add(user);
                await unitOfWork.CompleteAsync();

                //generating new JWT
                var token = await GenerateJwtToken(newUser);

                return Ok(new UserRegistrationResponseDto
                {
                    Success = true,
                    Token = token.JwtToken,
                    RefreshToken = token.RefreshToken
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

        /// <summary>
        ///  User login
        /// </summary>
        /// <param name="loginDto">Login Data Transfer Object</param>
        /// <returns>User Login response DTO</returns>
        [HttpPost]
        [Route("login")]
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
                var token = await  GenerateJwtToken(userExists);

                return Ok(new UserLoginResponseDto
                {
                    Success = true,
                    Token = token.JwtToken,
                    RefreshToken = token.RefreshToken
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
        ///  Refresh token endpoint
        /// </summary>
        /// <param name="tokenRequestDto">Token request Data Transfer Object</param>
        /// <returns>Refresh token DTO</returns>
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            if( ModelState.IsValid )
            {
                //model is valid

                //checking if token is valid
                var result = await VerifyToken(tokenRequestDto);

                if( result == null )
                {
                    return BadRequest(new UserLoginResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Token validation failed"
                        },
                    });
                }

                return Ok(result);

            } else
            {
                //model is not valid
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
        ///  Verifies tokens
        /// </summary>
        /// <param name="tokenRequestDto">Token request Data Transfer Object</param>
        /// <returns>Auth response DTO</returns>
        private async Task<AuthResponseDto> VerifyToken(TokenRequestDto tokenRequestDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {

                var tokenValidationParametersClone = this.tokenValidationParameters.Clone();
                tokenValidationParametersClone.ValidateLifetime = false;

                // checking token validity
                var principal = tokenHandler.ValidateToken(
                                                    tokenRequestDto.Token,
                                                    tokenValidationParametersClone,
                                                    out var validatedToken);
                

                // Check if the validated token is a valid jwt token (and not a random string)
                if( validatedToken is JwtSecurityToken jwtSecurityToken )
                {

                    // Check if the encryption algorithm used is correct
                    var isAlgorithmCorrect = jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256, 
                        StringComparison.CurrentCultureIgnoreCase);

                    if (!isAlgorithmCorrect) return null;
                }

                // Check if token expired
                var tokenExpiryTimestamp = 
                    long.Parse(
                        principal
                            .Claims
                            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)
                            .Value                  
                    );

                var tokenExpiryDateTime = UnixTimestampToDateTime(tokenExpiryTimestamp);

                // Checking if jwt token has expired
                if( tokenExpiryDateTime > DateTime.UtcNow )
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Jwt token has not expired."
                        }
                    };
                }

                Console.WriteLine("Token has expired");

                // Checking if refresh token exists
                var existingRefreshToken = await unitOfWork
                                                    .RefreshTokens
                                                    .GetByRefreshToken(tokenRequestDto.RefreshToken);

                if( existingRefreshToken == null )
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Invalid refresh token."
                        }
                    };
                }


                // Ckecking refresh token expiry date
                if (existingRefreshToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Refresh token has expired."
                        }
                    };
                }

                // Ckecking if refresh token has been already used
                if (existingRefreshToken.IsUsed)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Refresh token has been already used."
                        }
                    };
                }

                // Ckecking if refresh token has been already revoked
                if (existingRefreshToken.IsUsed)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Refresh token has been revoked."
                        }
                    };
                }

                // Getting JTI (jwt id), used in next check
                var jti = principal
                            .Claims
                            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)
                            .Value; 
    
                // Checking Jwt sent matches the one referenced by the refresh token
                if( existingRefreshToken.JwtId != jti )
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Refresh token reference does not match the jwt token"
                        }
                    };
                }

                // Start processing and get a new token
                existingRefreshToken.IsUsed = true;

                bool updateResult = await unitOfWork
                                            .RefreshTokens
                                            .MarkRefreshTokenAsUsed(existingRefreshToken);
                

                if(!updateResult)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Error in processing request."
                        }
                    };
                }
                
                await unitOfWork.CompleteAsync();

                // Getting the user in order to generate a new jwt token
                var dbUser = await userManager.FindByIdAsync(existingRefreshToken.UserId);
                if( dbUser == null )
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Error in processing request."
                        }
                    };
                }

                // Generating jwt token
                var token = await GenerateJwtToken(dbUser);

                return new AuthResponseDto
                {
                    Token = token.JwtToken,
                    Success = true,
                    RefreshToken = token.RefreshToken
                };

            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///  Converts a unix timestamp to DateTime object
        /// </summary>
        /// <param name="unixDate">Unix timestamp</param>
        /// <returns>DateTime object</returns>
        private DateTime UnixTimestampToDateTime(long unixDate)
        {
            // Sets date to 1 Jan, 1970
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            // Converts to DateTime
            dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();
            
            return dateTime;
        }

        /// <summary>
        ///  Generates new JWT
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>JWT string</returns>
        private async Task<TokenData> GenerateJwtToken(IdentityUser user)
        {
            //token handler
            var jwtHandler = new JwtSecurityTokenHandler();

            //getting security key
            var key = Encoding.ASCII.GetBytes(appSettings.JwtSecret);

            // Getting claims
            var claims = await GetAllValidClaims(user);

            //creating token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(appSettings.JwtExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //creating token based on token descripton
            var token = jwtHandler.CreateToken(tokenDescriptor);

            //converting token into a string
            var jwtToken = jwtHandler.WriteToken(token);

            //creating alphabet for generating random strings
            Alphabet alphabet = new Alphabet();
            alphabet
                .AddNumbers()
                .AddLettersUppercase();

            //generating refresh token
            var refreshToken = new RefreshToken
            {
                CreatedOn = DateTime.UtcNow,
                Token = $"{StringHelper.GenerateRandomString(25, alphabet)}_{Guid.NewGuid()}",
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false,
                Status = 1,
                JwtId = token.Id,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await unitOfWork.RefreshTokens.Add(refreshToken);
            await unitOfWork.CompleteAsync();

            var tokenData = new TokenData
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            };
            
            return tokenData;
        }

        /// <summary>
        ///  Gets all valid claims for a specified user
        /// </summary>
        /// <param name="user">Identity user</param>
        /// <returns>A list of claims</returns>
        private async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
        {

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Adding claims assigned to the user
            var userClaims = await userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Getting roles
            var userRoles = await userManager.GetRolesAsync(user);
            
            foreach( string userRole in userRoles )
            {
                var role = await roleManager.FindByNameAsync(userRole);

                if( role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await roleManager.GetClaimsAsync(role);

                    foreach( var roleClaim in roleClaims )
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }

        /// <summary>
        ///  Adds a user to a role
        /// </summary>
        /// <param name="userId">Identity user Id</param>
        /// <param name="roleDto">Role name</param>
        /// <returns>Generic DTO for handling success state and errors</returns>
        [HttpPost]
        [Route("{userId}/roles")]
        public async Task<IActionResult> AddToRole(string userId, RoleDto roleDto)
        {

            // Getting user object
            var user = await userManager.FindByIdAsync(userId);

            //Checking user
            if (user == null)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "User id does not exist."
                    }
                });
            }

            // Getting role object
            var role = await roleManager.FindByNameAsync(roleDto.RoleName);

            // Checking role
            if (role == null)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Role does not exist."
                    }
                });
            }

            // Adding role to user
            var result = await userManager.AddToRoleAsync(user, roleDto.RoleName);

            if (!result.Succeeded)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Cannot add role to user."
                    }
                });
            }

            return Ok(new GenericResponseDto
            {
                Success = true
            });
        }

        /// <summary>
        ///  Gets user roles
        /// </summary>
        /// <param name="userId">Identity User id</param>
        /// <returns>List of role names related to user</returns>
        [HttpGet]
        [Route("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            // Getting user object
            var user = await userManager.FindByIdAsync(userId);

            //Checking user
            if (user == null)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "User email does not exist."
                    }
                });
            }

            var roles = await userManager.GetRolesAsync(user);

            return Ok(roles);
        }

        /// <summary>
        ///  Removes a specified role from a user
        /// </summary>
        /// <param name="userId">Identity user Id</param>
        /// <param name="roleName">Role name to be removed</param>
        /// <returns>Generic DTO for handling success state and errors</returns>
        [HttpDelete]
        [Route("{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveFromRole(string userId, string roleName)
        {
            // Getting user object
            var user = await userManager.FindByIdAsync(userId);

            //Checking user
            if (user == null)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "User id does not exist."
                    }
                });
            }

            // Getting role object
            var role = await roleManager.FindByNameAsync(roleName);

            // Checking role
            if (role == null)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Role does not exist."
                    }
                });
            }

            // Removing role from user
            var result = await userManager.RemoveFromRoleAsync(user, roleName);

            if(!result.Succeeded)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Cannot remove role."
                    }
                });
            }

            return Ok(new GenericResponseDto
            {
                Success = true
            });
        }
    }
}
