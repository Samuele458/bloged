using BlogedWebapp.Data;
using BlogedWebapp.Models.Dtos.Generic;
using BlogedWebapp.Models.Dtos.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers.v1
{

    [Route("v{version:apiVersion}/roles")]
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly UserManager<IdentityUser> userManager;

        public RolesController(
                    IUnitOfWork unitOfWork,
                    RoleManager<IdentityRole> roleManager,
                    UserManager<IdentityUser> userManager) : base(unitOfWork)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        [HttpGet]
        [Route("")]
        public IActionResult GetAllRoles()
        {
            var roles = roleManager.Roles.ToList();

            return Ok(roles);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateRole(RoleDto roleDto)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleDto.RoleName);

            if(roleExists)
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

            if(!roleCreated.Succeeded)
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

        [HttpDelete]
        [Route("{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {

            // Getting role object
            var role = await roleManager.FindByNameAsync(roleName);

            // Checking if role exists or not
            if( role == null )
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

            if(!result.Succeeded)
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
