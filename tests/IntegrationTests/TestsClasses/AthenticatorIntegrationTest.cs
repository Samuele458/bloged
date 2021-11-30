using BlogedWebapp;
using BlogedWebapp.Entities;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using IntegrationTestsProject;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
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

        /// <summary>
        ///  Sets JWT Bearer Token in header
        /// </summary>
        /// <param name="token">JWT token</param>
        public void SetJwtBearer(string token)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        ///  Sets JWT Bearer Token in header
        /// </summary>
        /// <param name="authResponse">Response of register or login request (it contains information about JWT)</param>
        public async Task SetJwtBearer(HttpResponseMessage authResponse)
        {
            var responseString = await authResponse.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<AuthResponseDto>(responseString);

            SetJwtBearer(responseDto.Token);
        }

        /// <summary>
        ///  Register utility method
        /// </summary>
        /// <param name="requestDto">Register user Data Transfer Object</param>
        /// <returns>HTTP response message</returns>
        public async Task<HttpResponseMessage> Register(CreateUserRequestDto requestDto)
        {
            var response = await client.PostAsync($"{UriPrefix}/accounts/register",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return response;
        }

        /// <summary>
        ///  Login utility method
        /// </summary>
        /// <param name="requestDto">Login user Data Transfer Object</param>
        /// <returns>HTTP response message</returns>
        public async Task<HttpResponseMessage> Login(UserLoginRequestDto requestDto)
        {
            var response = await client.PostAsync($"{UriPrefix}/accounts/login",
                new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"));

            return response;
        }

    }
}
