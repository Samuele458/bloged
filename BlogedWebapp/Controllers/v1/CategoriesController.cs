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
    [Route("v{version:apiVersion}/blog/{blogId}/categories")]
    public class CategoriesController : BaseController
    {
        private readonly UserManager<AppUser> userManager;

        private readonly IAuthorizationService authorizationService;

        public CategoriesController(
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

        public async Task<IActionResult> CreateCategory(string blogId, CreateCategoryDto requestDto)
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

            Category alreadyExistsingCategory = await unitOfWork
                                                        .Categories
                                                        .GetByUrlName(blogId, requestDto.UrlName);
            if (alreadyExistsingCategory != null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Category already exists."
                    }
                });
            }

            Category newCategory = new Category
            {
                Title = requestDto.Title,
                UrlName = requestDto.UrlName,
                Description = requestDto.Description,
                OwnerId = blog.Id,
            };

            await unitOfWork.Categories.Add(blogId, newCategory);
            await unitOfWork.CompleteAsync();

            return Ok(newCategory);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(string blogId, string categoryId)
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

            Category category = await unitOfWork
                                        .Categories
                                        .GetById(Guid.Parse(categoryId));

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

            await unitOfWork.Categories.Delete(blogId, category);
            await unitOfWork.CompleteAsync();

            return Ok(new GenericResponseDto()
            {
                Success = true
            });
        }


        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(string blogId, string categoryId, UpdateCategoryRequestDto requestDto)
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

            Category category = await unitOfWork
                                        .Categories
                                        .GetById(Guid.Parse(categoryId));

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

            // Checking if new url name is valid
            Category alreadyExistsingCategory = await unitOfWork
                                            .Categories
                                            .GetByUrlName(blogId, requestDto.UrlName);
            if (alreadyExistsingCategory != null)
            {
                return BadRequest(new GenericResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        "Category already exists."
                    }
                });
            }

            try
            {
                // Updating model object
                EntityUpdater.Update(category, requestDto);
                await unitOfWork.Categories.Update(blogId, category);
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

            return Ok(category);
        }
    }
}
