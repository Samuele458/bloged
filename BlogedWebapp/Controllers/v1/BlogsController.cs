using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers.v1
{
    /// <summary>
    ///  Blogs controller
    /// </summary>
    [Route("v{version:apiVersion}/blogs")]
    public class BlogsController : BaseController
    {
        private readonly UserManager<AppUser> userManager;

        public BlogsController(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager
        )
            : base(unitOfWork)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            var blogs = await unitOfWork.Blogs.All();
            return Ok(blogs);
        }

       

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateBlog(CreateBlogRequestDto requestDto)
        {
            // Getting ID of user who made the request
            var IdentityId = User.Claims.FirstOrDefault(c => c.Type.Equals("Id"));

            if( IdentityId == null )
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Cannot detect user."
                    }
                });
            }

            // Checking if blog already exists
            Blog existingBlog = await unitOfWork.Blogs.GetByUrlName(requestDto.UrlName);
            if( existingBlog != null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Blog '"+existingBlog.UrlName+"' already exists."
                    }
                });
            }

            // Getting user
            AppUser user = await userManager.FindByIdAsync(IdentityId.Value);

            Blog newBlog = new Blog
            {
                Title = requestDto.Title,
                UrlName = requestDto.UrlName
            };

            // Set user as owner
            await unitOfWork.Blogs.SetBlogOwner(newBlog, user);
            await unitOfWork.CompleteAsync();

            return Ok(newBlog);
        }
    }
}
