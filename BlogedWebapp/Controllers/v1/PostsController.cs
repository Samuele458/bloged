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
    ///  Posts controller
    /// </summary>
    [Route("v{version:apiVersion}")]
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

        [HttpGet]
        [Route("post/{postId}")]
        public async Task<IActionResult> GetPostById(string postId)
        {
            var post = await this.unitOfWork.Posts.GetById(Guid.Parse(postId), ProjectionBehaviour.Full);

            if (post == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid postId."
                    }
                });
            }

            return Ok(post);
        }

        [HttpGet]
        [Route("blog/{blogUrl}/post/{postUrl}")]
        public async Task<IActionResult> GetPostByUrl(string blogUrl, string postUrl)
        {
            var blog = await this.unitOfWork.Blogs.GetByUrlName(blogUrl);

            if (blog == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid blog."
                    }
                });
            }

            var post = await this.unitOfWork.Posts.GetByUrlName(blog.Id, postUrl, ProjectionBehaviour.Full);

            if (post == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid post."
                    }
                });
            }

            return Ok(post);
        }


        /// <summary>
        ///  Gets all existing posts
        /// </summary>
        /// <returns>List of posts</returns>
        [HttpGet]
        [Route("blog/{blogId}/posts")]
        public async Task<IActionResult> GetPosts(string blogId)
        {
            var posts = await unitOfWork.Posts.All(blogId);
            return Ok(posts);
        }

        /// <summary>
        ///  Creates new post
        /// </summary>
        /// <param name="requestDto">Data Transfer Object for creating new post</param>
        /// <returns>Created post</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("blog/{blogId}/posts")]
        public async Task<IActionResult> CreatePost(string blogId, CreatePostRequestDto requestDto)
        {
            // Getting ID of user who made the request
            var IdentityId = User.Claims.FirstOrDefault(c => c.Type.Equals("Id"));

            // Checking if blog already exists
            Post existingPost = await unitOfWork.Posts.GetByUrlName(blogId, requestDto.UrlName);
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

            // Getting user who made the request
            AppUser user = await userManager.FindByIdAsync(IdentityId.Value);

            Blog blog = await unitOfWork.Blogs.GetById(new Guid(blogId));

            if (blog == null)
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

            var authorizationResult = await authorizationService
                                    .AuthorizeAsync(User, blog, "AtLeastBlogWriter");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            Category category = await unitOfWork
                            .Categories
                            .GetById(Guid.Parse(requestDto.CategoryId));

            if (category == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid categoryId."
                    }
                });
            }

            List<PostsTag> tags = new List<PostsTag>();

            if (requestDto.Tags != null)
            {
                foreach (var tagId in requestDto.Tags)
                {
                    Tag tag = await unitOfWork.Tags.GetById(Guid.Parse(tagId));


                    if (tag == null)
                    {
                        return BadRequest(new GenericResponseDto()
                        {
                            Success = false,
                            Errors = new List<string>()
                            {
                                "Invalid tagId."
                            }
                        });
                    }
                    else
                    {
                        tags.Add(new PostsTag()
                        {
                            TagId = tag.Id
                        });
                    }
                }
            }

            Post newPost = new Post
            {
                Title = requestDto.Title,
                UrlName = requestDto.UrlName,
                Content = requestDto.Content,
                Author = user,
                OwnerId = blog.Id,
                CategoryId = requestDto.CategoryId,
                Tags = tags
            };

            foreach (var tag in tags)
            {
                tag.PostId = newPost.Id;
            }

            await unitOfWork.Posts.Add(blog.Id, newPost);
            await unitOfWork.CompleteAsync();

            return Ok(newPost);
        }


        [HttpPut]
        [Route("post/{postId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> UpdatePost(string postId, UpdatePostRequestDto requestDto)
        {

            var post = await this.unitOfWork.Posts.GetById(Guid.Parse(postId), ProjectionBehaviour.Full);

            if (post == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid postId."
                    }
                });
            }

            Blog blog = await unitOfWork.Blogs.GetById(new Guid(post.OwnerId));

            if (blog == null)
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

            var authorizationResult = await authorizationService
                                    .AuthorizeAsync(User, blog, "AtLeastBlogWriter");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            if (requestDto.CategoryId != null)
            {
                Category category = await unitOfWork
                     .Categories
                     .GetById(Guid.Parse(requestDto.CategoryId));

                if (category == null)
                {
                    return BadRequest(new GenericResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>()
                    {
                        "Invalid categoryId."
                    }
                    });
                }
            }

            post.Title = requestDto.Title ?? post.Title;
            post.UrlName = requestDto.UrlName ?? post.UrlName;
            post.Content = requestDto.Content ?? post.Content;

            EntityUpdater.Update(post, requestDto);
            await unitOfWork.Posts.Update(blog.Id, post);
            await unitOfWork.CompleteAsync();

            return Ok(post);
        }



        [HttpPost]
        [Route("post/{postId}/tags")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddTagToPost(string postId, AddTagToPostRequestDto requestDto)
        {
            var post = await this.unitOfWork.Posts.GetById(Guid.Parse(postId), ProjectionBehaviour.Full);

            if (post == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid postId."
                    }
                });
            }

            Blog blog = await unitOfWork.Blogs.GetById(new Guid(post.OwnerId));

            if (blog == null)
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

            var authorizationResult = await authorizationService
                                    .AuthorizeAsync(User, blog, "AtLeastBlogWriter");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var tag = await unitOfWork.Tags.GetById(Guid.Parse(requestDto.TagId));

            if (tag == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid tagId"
                    }
                });
            }

            if (post.Tags == null)
            {
                post.Tags = new List<PostsTag>();
            }

            if (post.Tags.Where(t => t.Tag.Id == tag.Id).FirstOrDefault() != null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Tag already added to post."
                    }
                });
            }

            //var updatablePost = await unitOfWork.Posts.Update(blog.Id, post);

            /*updatablePost.Tags.Add(new PostsTag()
            {
                PostId = post.Id,
                TagId = tag.Id
            });*/

            await unitOfWork.Posts.AddTagToPost(post, tag);

            await unitOfWork.CompleteAsync();

            return Ok(new GenericResponseDto()
            {
                Success = true
            });
        }

        [HttpDelete]
        [Route("post/{postId}/tag/{tagId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RemoveTagFromPost(string postId, string tagId)
        {
            var post = await this.unitOfWork.Posts.GetById(Guid.Parse(postId), ProjectionBehaviour.Full);

            if (post == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid postId."
                    }
                });
            }

            Blog blog = await unitOfWork.Blogs.GetById(new Guid(post.OwnerId));

            if (blog == null)
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

            var authorizationResult = await authorizationService
                                    .AuthorizeAsync(User, blog, "AtLeastBlogWriter");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var tag = await unitOfWork.Tags.GetById(Guid.Parse(tagId));

            if (tag == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid tagId"
                    }
                });
            }

            if (post.Tags.Where(t => t.Tag.Id == tag.Id).FirstOrDefault() == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Tag not present in post."
                    }
                });
            }

            //var updatablePost = await unitOfWork.Posts.Update(blog.Id, post);

            PostsTag tagToRemove = post
                                    .Tags
                                    .Where(t => t.Tag.Id == tag.Id)
                                    .FirstOrDefault();
            unitOfWork
                        .Posts
                        .RemoveTagFromPost(tagToRemove);

            await unitOfWork.CompleteAsync();

            return Ok(new GenericResponseDto()
            {
                Success = true
            });
        }
    }
}
