using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
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

        private readonly IAuthorizationService authorizationService;

        public BlogsController(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IAuthorizationService authorizationService
        )
            : base(unitOfWork)
        {
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        /// <summary>
        ///  Gets all existing blogs
        /// </summary>
        /// <returns>List of blogs</returns>
        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            var blogs = await unitOfWork.Blogs.All();
            return Ok(blogs);
        }


        /// <summary>
        ///  Creates new blog
        /// </summary>
        /// <param name="requestDto">Data Transfer Object for creating new blog</param>
        /// <returns>Created blog</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateBlog(CreateBlogRequestDto requestDto)
        {
            // Getting ID of user who made the request
            var IdentityId = User.Claims.FirstOrDefault(c => c.Type.Equals("Id"));

            // Checking if blog already exists
            Blog existingBlog = await unitOfWork.Blogs.GetByUrlName(requestDto.UrlName);
            if (existingBlog != null)
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


        /// <summary>
        ///  Update an existing blog
        /// </summary>
        /// <param name="id">Blog ID</param>
        /// <param name="requestDto">Data Trasfer Object containing updated fields</param>
        /// <returns>Updated blog</returns>
        [HttpPut]
        [Route("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateBlog(Guid id, UpdateBlogRequestDto requestDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Invalid parameters."
                    }
                });
            }

            Blog blog = await unitOfWork.Blogs.GetById(id);

            // Checks if blog exists
            if (blog == null)
            {
                return NotFound(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Blog does not exist."
                    }
                });
            }

            // Getting ID of user who made the request
            var UserId = User.Claims.FirstOrDefault(c => c.Type.Equals("Id")).Value;

            var authorizationResult = await authorizationService
                                                .AuthorizeAsync(User, blog, "AllowedToUseBlog");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            try
            {
                // Updating model object
                EntityUpdater.Update(blog, requestDto);
                await unitOfWork.Blogs.Update(blog);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                return BadRequest(new GenericResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Invalid parameters."
                    }
                });
            }

            return Ok(blog);
        }
    }
}
