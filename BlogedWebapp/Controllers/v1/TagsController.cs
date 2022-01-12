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
using System.Threading.Tasks;

namespace BlogedWebapp.Controllers.v1
{
    [Route("v{version:apiVersion}/blog/{blogId}/tags")]
    public class TagsController : BaseController
    {
        private readonly UserManager<AppUser> userManager;

        private readonly IAuthorizationService authorizationService;

        public TagsController(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IAuthorizationService authorizationService
        )
            : base(unitOfWork)
        {
            this.userManager = userManager;
            this.authorizationService = authorizationService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> CreateTag(string blogId, CreateTagRequestDto requestDto)
        {
            Blog blog = await unitOfWork.Blogs.GetById(Guid.Parse(blogId));

            if (blog == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid blogId."
                    }
                });
            }

            var authorizationResult = await authorizationService
                                    .AuthorizeAsync(User, blog, "AtLeastBlogWriter");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            Tag alreadyExistingTag = await unitOfWork
                                                .Tags
                                                .GetByUrlName(blogId, requestDto.UrlName);

            if (alreadyExistingTag != null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Tag already exists."
                    }
                });
            }

            Tag newTag = new Tag
            {
                Title = requestDto.Title,
                UrlName = requestDto.UrlName,
                Description = requestDto.Description,
                OwnerId = blog.Id,
            };

            await unitOfWork.Tags.Add(blogId, newTag);
            await unitOfWork.CompleteAsync();

            return Ok(newTag);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("{tagId}")]
        public async Task<IActionResult> DeleteTag(string blogId, string tagId)
        {
            Blog blog = await unitOfWork.Blogs.GetById(Guid.Parse(blogId));

            if (blog == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid blogId."
                    }
                });
            }

            var authorizationResult = await authorizationService
                                    .AuthorizeAsync(User, blog, "AtLeastBlogWriter");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            Tag tag = await unitOfWork
                                    .Tags
                                    .GetById(Guid.Parse(tagId));

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

            await unitOfWork.Tags.Delete(blogId, tag);
            await unitOfWork.CompleteAsync();

            return Ok(new GenericResponseDto()
            {
                Success = true
            });
        }


        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("{tagId}")]
        public async Task<IActionResult> UpdateTag(string blogId, string tagId, UpdateCategoryRequestDto requestDto)
        {

            Blog blog = await unitOfWork.Blogs.GetById(Guid.Parse(blogId));

            if (blog == null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Invalid blogId."
                    }
                });
            }

            var authorizationResult = await authorizationService
                                    .AuthorizeAsync(User, blog, "AtLeastBlogWriter");

            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            Tag tag = await unitOfWork
                                    .Tags
                                    .GetById(Guid.Parse(tagId));

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

            // Checking if new url name is valid
            Tag alreadyExistingTag = await unitOfWork
                                            .Tags
                                            .GetByUrlName(blogId, requestDto.UrlName);

            if (alreadyExistingTag != null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Tag already exists."
                    }
                });
            }

            try
            {
                // Updating model object
                EntityUpdater.Update(tag, requestDto);
                await unitOfWork.Tags.Update(blogId, tag);
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

            return Ok(tag);
        }
    }
}
