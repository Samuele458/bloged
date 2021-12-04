using BlogedWebapp;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using IntegrationTestsProject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class BlogsControllerTests : AthenticatorIntegrationTest
    {
        public BlogsControllerTests(TestingWebAppFactory<Startup> factory)
                : base(factory)
        {

        }

        [Fact]
        public async void CreateBlog_RequestWithoutAuthentication_Unauthorized()
        {
            Reset();

            var requestDto = new CreateBlogRequestDto
            {
                UrlName = "tech-blog",
                Title = "TechBlog"
            };

            var res = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            Assert.True(res.StatusCode != HttpStatusCode.OK);
            Assert.Null(context.Set<Blog>().FirstOrDefault(b => b.UrlName.Equals("tech-blog")));
        }

        [Fact]
        public async void CreateBlog_ValidRequest_BlogCreated()
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

            var requestDto = new CreateBlogRequestDto
            {
                UrlName = "tech-blog",
                Title = "TechBlog"
            };

            var res = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            Assert.True(res.StatusCode == HttpStatusCode.OK);
            Assert.NotNull(context.Set<Blog>().FirstOrDefault(b => b.UrlName.Equals("tech-blog")));
        }

        
        [Fact]
        public async void DeleteBlog_ValidRequestFromAdmin_BlogDeleted()
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

            var requestDto = new CreateBlogRequestDto
            {
                UrlName = "tech-blog",
                Title = "TechBlog"
            };

            var postRes = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var blog = JsonConvert.DeserializeObject<Blog>(await postRes.Content.ReadAsStringAsync());

            var deleteRes = await client.DeleteAsync($"{UriPrefix}/blogs/{blog.Id}");

            Assert.True(deleteRes.StatusCode == HttpStatusCode.OK);
            Assert.Null(context.Set<Blog>().FirstOrDefault(b => b.UrlName.Equals("tech-blog")));
        }

        [Fact]
        public async void DeleteBlog_ValidRequestFromBlogOwner_BlogDeleted()
        {
            Reset();

            // Creating superadmin user
            await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            //Creating normal user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele1@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele4589",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(authResponse);

            var requestDto = new CreateBlogRequestDto
            {
                UrlName = "tech-blog",
                Title = "TechBlog"
            };

            var postRes = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var blog = JsonConvert.DeserializeObject<Blog>(await postRes.Content.ReadAsStringAsync());

            var deleteRes = await client.DeleteAsync($"{UriPrefix}/blogs/{blog.Id}");

            Assert.True(deleteRes.StatusCode == HttpStatusCode.OK);
            Assert.Null(context.Set<Blog>().FirstOrDefault(b => b.UrlName.Equals("tech-blog")));
        }

        [Fact]
        public async void DeleteBlog_RequestWithoutAuthentication_Unauthorized()
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

            var requestDto = new CreateBlogRequestDto
            {
                UrlName = "tech-blog",
                Title = "TechBlog"
            };

            var postRes = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            var blog = JsonConvert.DeserializeObject<Blog>(await postRes.Content.ReadAsStringAsync());

            RemoveJwtBearer();

            var deleteRes = await client.DeleteAsync($"{UriPrefix}/blogs/{blog.Id}");

            Assert.True(deleteRes.StatusCode != HttpStatusCode.OK);
            Assert.NotNull(context.Set<Blog>().FirstOrDefault(b => b.UrlName.Equals("tech-blog")));
        }

        [Fact]
        public async void DeleteBlog_RequestWithoutAuthRequirements_Unauthorized()
        {
            Reset();

            // Creating superadmin user
            var adminAuthResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            //Creating normal user
            var normalUserAuthResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(adminAuthResponse);

            var requestDto = new CreateBlogRequestDto
            {
                UrlName = "tech-blog",
                Title = "TechBlog"
            };

            var postRes = await client.PostAsync($"{UriPrefix}/blogs",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));
            
            var blog = JsonConvert.DeserializeObject<Blog>(await postRes.Content.ReadAsStringAsync());

            await SetJwtBearer(normalUserAuthResponse);

            var deleteRes = await client.DeleteAsync($"{UriPrefix}/blogs/{blog.Id}");

            Assert.True(deleteRes.StatusCode != HttpStatusCode.OK);
            Assert.NotNull(context.Set<Blog>().FirstOrDefault(b => b.UrlName.Equals("tech-blog")));
        }
    }
}
