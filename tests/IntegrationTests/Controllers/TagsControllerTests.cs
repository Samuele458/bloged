using BlogedWebapp;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using IntegrationTestsProject;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class TagsControllerTests : AthenticatorIntegrationTest
    {
        public TagsControllerTests(TestingWebAppFactory<Startup> factory)
        : base(factory)
        {

        }

        public async Task<HttpResponseMessage> CreateBlog(CreateBlogRequestDto requestDto)
        {
            var res = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return res;
        }

        [Fact]
        public async void CreateTag_ValidRequest_TagCreated()
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

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var requestDto = new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            };

            var tagCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/tags",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            Assert.True(tagCreationRes.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(context.Set<Tag>().FirstOrDefault(b => b.UrlName.Equals("test-tag") && b.OwnerId.Equals(blog.Id)));

        }

        [Fact]
        public async void CreateTag_CreatingTagWithDuplicateUrlName_BadRequest()
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

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var requestDto = new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            };

            var tagCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/tags",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            // Creating for the 2nd time tha same object
            tagCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/tags",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            //var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            Assert.True(tagCreationRes.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(
                context
                    .Set<Tag>()
                    .Where(b => b.UrlName.Equals("test-tag") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 1
            );

        }

        [Fact]
        public async void CreateTag_CreatingTagWithWrongBlogId_BadRequest()
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

            await SetJwtBearer(authResponse);

            var requestDto = new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            };

            var tagCreationRes = await client.PostAsync($"{UriPrefix}/blog/wrong-blog-id/tags",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            //var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            Assert.True(tagCreationRes.StatusCode == HttpStatusCode.InternalServerError);
            Assert.True(
                context
                    .Set<Tag>()
                    .Where(b => b.UrlName.Equals("test-tag"))
                    .ToList().Count == 0
            );

        }

        [Fact]
        public async void CreateTag_RequestWithoutAuthentication_Unauthorized()
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

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            // Removing authentication
            this.RemoveJwtBearer();

            var requestDto = new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            };

            var tagCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/tags",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            Assert.True(tagCreationRes.StatusCode == HttpStatusCode.Unauthorized);
            Assert.Null(context.Set<Tag>().FirstOrDefault(b => b.UrlName.Equals("test-tag") && b.OwnerId.Equals(blog.Id)));

        }

        [Fact]
        public async void UpdateTag_ValidRequest_TagUpdated()
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

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var createRequestDto = new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            };

            var tagCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/tags",
                new StringContent(JsonConvert.SerializeObject(createRequestDto), Encoding.UTF8, "application/json"));

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            var updateRequestDto = new UpdateTagRequestDto
            {
                Title = "TestTag2",
                UrlName = "test-tag-2"
            };

            var tagUpdateRes = await client.PutAsync($"{UriPrefix}/blog/{blog.Id}/tags/{tag.Id}",
                new StringContent(JsonConvert.SerializeObject(updateRequestDto), Encoding.UTF8, "application/json"));

            var a = context
                    .Set<Tag>()
                    .ToList();


            Assert.True(tagUpdateRes.StatusCode == HttpStatusCode.OK);
            Assert.True(
                context
                    .Set<Tag>()
                    .Where(b => b.UrlName.Equals("test-tag") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 0
            );

            Assert.True(
                context
                    .Set<Tag>()
                    .Where(b => b.UrlName.Equals("test-tag-2") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 1
            );
        }


        [Fact]
        public async void DeleteTag_ValidRequest_TagDeleted()
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

            await SetJwtBearer(authResponse);

            var blogCreationRes = await CreateBlog(new CreateBlogRequestDto
            {
                Title = "TestBlog",
                UrlName = "test-blog"
            });

            var blog = JsonConvert.DeserializeObject<Blog>(await blogCreationRes.Content.ReadAsStringAsync());

            var createRequestDto = new CreateTagRequestDto
            {
                Title = "TestTag",
                UrlName = "test-tag"
            };

            var tagCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/tags",
                new StringContent(JsonConvert.SerializeObject(createRequestDto), Encoding.UTF8, "application/json"));

            var tag = JsonConvert.DeserializeObject<Tag>(await tagCreationRes.Content.ReadAsStringAsync());

            Assert.True(
                context
                    .Set<Tag>()
                    .Where(b => b.UrlName.Equals("test-tag") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 1
            );

            var deleteRes = await client.DeleteAsync($"{UriPrefix}/blog/{blog.Id}/tags/{tag.Id}");

            Assert.True(deleteRes.StatusCode == HttpStatusCode.OK);
            Assert.True(
                context
                    .Set<Tag>()
                    .Where(b => b.UrlName.Equals("test-tag") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 0
            );

        }
    }
}
