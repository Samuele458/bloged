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
    public class CategoriesControllerTests : AthenticatorIntegrationTest
    {
        public CategoriesControllerTests(TestingWebAppFactory<Startup> factory)
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
        public async void CreateCategory_ValidRequest_CategoryCreated()
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

            var requestDto = new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            };

            var categoryCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/categories",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            Assert.True(categoryCreationRes.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(context.Set<Category>().FirstOrDefault(b => b.UrlName.Equals("test-category") && b.OwnerId.Equals(blog.Id)));

        }

        [Fact]
        public async void CreateCategory_CreatingCategoryWithDuplicateUrlName_BadRequest()
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

            var requestDto = new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            };

            var categoryCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/categories",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            // Creating for the 2nd time tha same object
            categoryCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/categories",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            //var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            Assert.True(categoryCreationRes.StatusCode == HttpStatusCode.BadRequest);
            Assert.True(
                context
                    .Set<Category>()
                    .Where(b => b.UrlName.Equals("test-category") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 1
            );

        }

        [Fact]
        public async void CreateCategory_CreatingCategoryWithWrongBlogId_BadRequest()
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

            var requestDto = new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            };

            var categoryCreationRes = await client.PostAsync($"{UriPrefix}/blog/wrong-blog-id/categories",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            //var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            Assert.True(categoryCreationRes.StatusCode == HttpStatusCode.InternalServerError);
            Assert.True(
                context
                    .Set<Category>()
                    .Where(b => b.UrlName.Equals("test-category"))
                    .ToList().Count == 0
            );

        }

        [Fact]
        public async void CreateCategory_RequestWithoutAuthentication_Unauthorized()
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

            var requestDto = new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            };

            var categoryCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/categories",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            Assert.True(categoryCreationRes.StatusCode == HttpStatusCode.Unauthorized);
            Assert.Null(context.Set<Category>().FirstOrDefault(b => b.UrlName.Equals("test-category") && b.OwnerId.Equals(blog.Id)));

        }

        [Fact]
        public async void UpdateCategory_ValidRequest_CategoryUpdated()
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

            var createRequestDto = new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            };

            var categoryCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/categories",
                new StringContent(JsonConvert.SerializeObject(createRequestDto), Encoding.UTF8, "application/json"));

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            var updateRequestDto = new UpdateCategoryRequestDto
            {
                Title = "TestCategory2",
                UrlName = "test-category-2"
            };

            var categoryUpdateRes = await client.PutAsync($"{UriPrefix}/blog/{blog.Id}/categories/{category.Id}",
                new StringContent(JsonConvert.SerializeObject(updateRequestDto), Encoding.UTF8, "application/json"));

            var a = context
                    .Set<Category>()
                    .ToList();


            Assert.True(categoryUpdateRes.StatusCode == HttpStatusCode.OK);
            Assert.True(
                context
                    .Set<Category>()
                    .Where(b => b.UrlName.Equals("test-category") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 0
            );

            Assert.True(
                context
                    .Set<Category>()
                    .Where(b => b.UrlName.Equals("test-category-2") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 1
            );
        }


        [Fact]
        public async void DeleteCategory_ValidRequest_CategoryDeleted()
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

            var createRequestDto = new CreateCategoryDto
            {
                Title = "TestCategory",
                UrlName = "test-category"
            };

            var categoryCreationRes = await client.PostAsync($"{UriPrefix}/blog/{blog.Id}/categories",
                new StringContent(JsonConvert.SerializeObject(createRequestDto), Encoding.UTF8, "application/json"));

            var category = JsonConvert.DeserializeObject<Category>(await categoryCreationRes.Content.ReadAsStringAsync());

            Assert.True(
                context
                    .Set<Category>()
                    .Where(b => b.UrlName.Equals("test-category") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 1
            );

            var deleteRes = await client.DeleteAsync($"{UriPrefix}/blog/{blog.Id}/categories/{category.Id}");

            Assert.True(deleteRes.StatusCode == HttpStatusCode.OK);
            Assert.True(
                context
                    .Set<Category>()
                    .Where(b => b.UrlName.Equals("test-category") && b.OwnerId.Equals(blog.Id))
                    .ToList().Count == 0
            );

        }
    }
}
