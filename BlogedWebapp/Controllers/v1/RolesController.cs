using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Generic;
using BlogedWebapp.Models.Dtos.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers.v1
{

    [Route("v{version:apiVersion}/roles")]
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly UserManager<AppUser> userManager;

        public RolesController(
                    IUnitOfWork unitOfWork,
                    RoleManager<IdentityRole> roleManager,
                    UserManager<AppUser> userManager) : base(unitOfWork)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        ///  Gets all existing roles
        /// </summary>
        /// <returns>List of all existing roles</returns>
        [HttpGet]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAllRoles()
        {
            var roles = roleManager.Roles.ToList();

            return Ok(roles);
        }

        /// <summary>
        ///  Creates a new role
        /// </summary>
        /// <param name="roleDto">Role Data Transfer Object containing role information</param>
        /// <returns>Generic DTO for handling success state and errors</returns>
        [HttpPost]
        [Route("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminOrSuperadmin")]
        public async Task<IActionResult> CreateRole(RoleDto roleDto)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleDto.RoleName);

            if (roleExists)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Role already exists"
                    }
                });
            }

            var roleCreated = await roleManager.CreateAsync(new IdentityRole(roleDto.RoleName));

            if (!roleCreated.Succeeded)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Cannot create role"
                    }
                });
            }

            return Ok(new GenericResponseDto
            {
                Success = true
            });

        }


        /// <summary>
        ///  Deletes a specified role.
        ///  The role will also be removed from every user who is attached to
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Generic DTO for handling success state and errors</returns>
        [HttpDelete]
        [Route("{roleName}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminOrSuperadmin")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {

            // Getting role object
            var role = await roleManager.FindByNameAsync(roleName);

            // Checking if role exists or not
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

            // Deleting role
            var result = await roleManager.DeleteAsync(role);

            if (!result.Succeeded)
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

            return Ok(new GenericResponseDto
            {
                Success = true
            });
        }

    }
}
