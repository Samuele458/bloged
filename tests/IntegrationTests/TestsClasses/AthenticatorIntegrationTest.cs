using BlogedWebapp;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using IntegrationTestsProject;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class AthenticatorIntegrationTest : BaseIntegrationTest
    {
        protected UserManager<AppUser> userManager;

        protected RoleManager<IdentityRole> roleManager;

        public AthenticatorIntegrationTest(TestingWebAppFactory<Startup> factory)
            : base(factory)
        {
            this.userManager = serviceProvider.GetService<UserManager<AppUser>>();

            this.roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
        }


        public async Task<HttpResponseMessage> Register(CreateUserRequestDto requestDto)
        {
            var response = await client.PostAsync($"{UriPrefix}/accounts/register",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return response;
        }

        public async Task<HttpResponseMessage> Login(UserLoginRequestDto requestDto)
        {
            var response = await client.PostAsync($"{UriPrefix}/accounts/login",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return response;
        }

        //protected bool createUser
    }
}
