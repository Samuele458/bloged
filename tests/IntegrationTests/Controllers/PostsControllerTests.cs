using BlogedWebapp;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using IntegrationTestsProject;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class PostsControllerTests : AthenticatorIntegrationTest
    {
        public PostsControllerTests(TestingWebAppFactory<Startup> factory)
            : base(factory)
        {

        }

        public async Task<HttpResponseMessage> CreateBlog(CreateBlogRequestDto requestDto)
        {
            var res = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return res;
        }

        public async Task<HttpResponseMessage> CreateCategory(string blogId, CreateCategoryDto requestDto)
        {
            var res = await client.PostAsync($"{UriPrefix}/blog/{blogId}/categories",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return res;
        }

        public async Task<HttpResponseMessage> CreateTag(string blogId, CreateTagRequestDto requestDto)
        {
            var res = await client.PostAsync($"{UriPrefix}/blog/{blogId}/tags",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return res;
        }

        [Fact]
        public async void CreatePost_ValidRequest_PostCreated()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var requestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var b = await postCreationRes.Content.ReadAsStringAsync();

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            var a = context
                .Set<Post>()
                .Include(b => b.Tags)
                .FirstOrDefault(b => b.UrlName.Equals("post-1"));

            Assert.True(postCreationRes.StatusCode == HttpStatusCode.OK);
            Assert.True(
                context
                    .Set<Post>()
                    .Where(b =>
                        b.UrlName.Equals("post-1") &&
                        b.OwnerId.Equals(blog.Id) &&
                        b.CategoryId == category.Id
                    ).ToList().Count == 1
            );
        }

        [Fact]
        public async void CreatePost_RequestWithoutAuthentication_Unauthorized()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var requestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            RemoveJwtBearer();

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            Assert.True(postCreationRes.StatusCode == HttpStatusCode.Unauthorized);
            Assert.True(
                context
                    .Set<Post>()
                    .Where(b =>
                        b.UrlName.Equals("post-1") &&
                        b.OwnerId.Equals(blog.Id) &&
                        b.CategoryId == category.Id
                    ).ToList().Count == 0
            );
        }


        [Fact]
        public async void CreatePost_RequestFromUnauthorizedAccount_Unauthorized()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var requestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            RemoveJwtBearer();

            // AUthentication from another account
            authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele2@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele4589",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(authResponse);

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            Assert.True(postCreationRes.StatusCode == HttpStatusCode.Unauthorized);
            Assert.True(
                context
                    .Set<Post>()
                    .Where(b =>
                        b.UrlName.Equals("post-1") &&
                        b.OwnerId.Equals(blog.Id) &&
                        b.CategoryId == category.Id
                    ).ToList().Count == 0
            );
        }

        [Fact]
        public async void GetPost_ByUrlNames_PostDtoReturned()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var requestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            var postObjRes = await client.GetAsync($"{UriPrefix}/blog/{blog.UrlName}/post/{post.UrlName}");

            post = JsonConvert.DeserializeObject<Post>(await postObjRes.Content.ReadAsStringAsync());

            System.Diagnostics.Debug.WriteLine("Response: " + await postObjRes.Content.ReadAsStringAsync());

            Assert.True(postCreationRes.StatusCode == HttpStatusCode.OK);
            Assert.Equal(post.Title, requestDto.Title);
            Assert.Equal(post.UrlName, requestDto.UrlName);
            Assert.Equal(post.Content, requestDto.Content);
            Assert.Equal(post.AuthorId, requestDto.AuthorId);
            Assert.Equal(post.CategoryId, requestDto.CategoryId);
        }

        [Fact]
        public async void GetPost_ById_PostDtoReturned()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var requestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            var postObjRes = await client.GetAsync($"{UriPrefix}/post/{post.Id}");

            post = JsonConvert.DeserializeObject<Post>(await postObjRes.Content.ReadAsStringAsync());

            System.Diagnostics.Debug.WriteLine("Response: " + await postObjRes.Content.ReadAsStringAsync());

            Assert.True(postCreationRes.StatusCode == HttpStatusCode.OK);
            Assert.Equal(post.Title, requestDto.Title);
            Assert.Equal(post.UrlName, requestDto.UrlName);
            Assert.Equal(post.Content, requestDto.Content);
            Assert.Equal(post.AuthorId, requestDto.AuthorId);
            Assert.Equal(post.CategoryId, requestDto.CategoryId);
        }

        [Fact]
        public async void UpdatePost_ValidRequest_PostUpdated()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var creationRequestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(creationRequestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            var updateRequestDto = new UpdatePostRequestDto()
            {
                Title = "Title v2",
                UrlName = "title-v2",
            };

            var postUpdateRes = await client.PutAsync($"{UriPrefix}/post/{post.Id}",
                new StringContent(JsonConvert.SerializeObject(updateRequestDto), Encoding.UTF8, "application/json"));

            var a = await postUpdateRes.Content.ReadAsStringAsync();

            post = JsonConvert.DeserializeObject<Post>(await postUpdateRes.Content.ReadAsStringAsync());

            Assert.True(postCreationRes.StatusCode == HttpStatusCode.OK);
            Assert.Equal(post.Title, updateRequestDto.Title);
            Assert.Equal(post.UrlName, updateRequestDto.UrlName);
            Assert.Equal(post.Content, creationRequestDto.Content);
            Assert.Equal(post.AuthorId, creationRequestDto.AuthorId);
            Assert.Equal(post.CategoryId, creationRequestDto.CategoryId);
        }

        [Fact]
        public async void UpdatePost_RequestWithoutAuthentication_Unauthorized()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var creationRequestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(creationRequestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            RemoveJwtBearer();

            var updateRequestDto = new UpdatePostRequestDto()
            {
                Title = "Title v2",
                UrlName = "title-v2",
            };

            var postUpdateRes = await client.PutAsync($"{UriPrefix}/post/{post.Id}",
                new StringContent(JsonConvert.SerializeObject(updateRequestDto), Encoding.UTF8, "application/json"));

            Assert.True(postUpdateRes.StatusCode == HttpStatusCode.Unauthorized);
            Assert.NotNull(context.Set<Post>().Where(p => p.Title == creationRequestDto.Title).FirstOrDefault());
            Assert.Null(context.Set<Post>().Where(p => p.Title == updateRequestDto.Title).FirstOrDefault());

        }

        [Fact]
        public async void UpdatePost_RequestFromUnauthorizedAccount_Unauthorized()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var creationRequestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(creationRequestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            RemoveJwtBearer();

            authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele2@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele4582",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(authResponse);

            var updateRequestDto = new UpdatePostRequestDto()
            {
                Title = "Title v2",
                UrlName = "title-v2",
            };

            var postUpdateRes = await client.PutAsync($"{UriPrefix}/post/{post.Id}",
                new StringContent(JsonConvert.SerializeObject(updateRequestDto), Encoding.UTF8, "application/json"));

            Assert.True(postUpdateRes.StatusCode == HttpStatusCode.Unauthorized);
            Assert.NotNull(context.Set<Post>().Where(p => p.Title == creationRequestDto.Title).FirstOrDefault());
            Assert.Null(context.Set<Post>().Where(p => p.Title == updateRequestDto.Title).FirstOrDefault());

        }

        [Fact]
        public async void AddTagToPost_ValidRequest_PostUpdated()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var creationRequestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(creationRequestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            var postAddTagRes = await client.PostAsync($"{UriPrefix}/post/{post.Id}/tags",
                new StringContent(JsonConvert.SerializeObject(new AddTagToPostRequestDto() { TagId = tag.Id }), Encoding.UTF8, "application/json"));


            Assert.True(postAddTagRes.StatusCode == HttpStatusCode.OK);
            Assert.True(context.Set<Post>().Include(p => p.Tags).Where(p => p.Id == post.Id).FirstOrDefault().Tags.Count == 1);
        }


        [Fact]
        public async void RemoveTagFromPost_ValidRequest_PostUpdated()
        {
            Reset();

            // Creating superadmin user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            var authresponseObj = JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync());

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var categoryCreationRes = await CreateCategory(blog.Id, new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            });

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var tagCreationRes = await CreateTag(blog.Id, new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            });

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var creationRequestDto = new CreatePostRequestDto()
            {
                Title = "Post 1",
                UrlName = "post-1",
                Content = "Post content",
                AuthorId = authresponseObj.UserId,
                CategoryId = category.Id,
                Tags = new List<string>()
                {
                    tag.Id
                }
            };

            var postCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/posts",
                new StringContent(JsonConvert.SerializeObject(creationRequestDto), Encoding.UTF8, "application/json"));

            var post = JsonConvert.DeserializeObject<Post>(await postCreationRes.Content.ReadAsStringAsync());

            var postRemoveTagRes = await client.DeleteAsync($"{UriPrefix}/post/{post.Id}/tag/{tag.Id}");

            var a = context.Set<Post>().Include(p => p.Tags).Where(p => p.Id == post.Id).FirstOrDefault();

            Assert.True(postRemoveTagRes.StatusCode == HttpStatusCode.OK);
            //Assert.True(.Tags.Count == 0);
        }

    }
}
