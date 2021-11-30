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
    ///  Posts controller
    /// </summary>
    [Route("v{version:apiVersion}/posts")]
    public class PostsController : BaseController
    {
        private readonly UserManager<AppUser> userManager;

        private readonly IAuthorizationService authorizationService;

        public PostsController(
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
        ///  Gets all existing posts
        /// </summary>
        /// <returns>List of posts</returns>
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await unitOfWork.Posts.All();
            return Ok(posts);
        }

        /// <summary>
        ///  Creates new post
        /// </summary>
        /// <param name="requestDto">Data Transfer Object for creating new post</param>
        /// <returns>Created post</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreatePost(CreatePostRequestDto requestDto)
        {
            // Getting ID of user who made the request
            var IdentityId = User.Claims.FirstOrDefault(c => c.Type.Equals("Id"));

            // Checking if blog already exists
            Post existingPost = await unitOfWork.Posts.GetByUrlName(requestDto.UrlName);
            if (existingPost != null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Post '"+existingPost.UrlName+"' already exists."
                    }
                });
            }

            // Getting user
            AppUser user = await userManager.FindByIdAsync(IdentityId.Value);

            // Getting author
            AppUser author = await userManager.FindByIdAsync(requestDto.AuthorId);

            if( author == null )
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid AuthorId"
                    }
                });
            }

            Blog blog = await unitOfWork.Blogs.GetById(new Guid(requestDto.BlogId));

            if( blog == null )
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid BlogId"
                    }
                });
            }

            Post newPost = new Post
            {
                Title = requestDto.Title,
                UrlName = requestDto.UrlName,
                Content = requestDto.Content,
                Author = author,
                Owner = blog
            };

            await unitOfWork.Posts.Add(newPost);
            await unitOfWork.CompleteAsync();

            return Ok(newPost);
        }
    }
}
