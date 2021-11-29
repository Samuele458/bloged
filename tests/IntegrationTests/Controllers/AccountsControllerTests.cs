using BlogedWebapp;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using IntegrationTestsProject;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class AccountsControllerTests : AthenticatorIntegrationTest
    {

        public AccountsControllerTests(TestingWebAppFactory<Startup> factory)
            : base(factory)
        {

        }

        [Fact]
        public async Task Register_PassingValidFields_UserCreated()
        {
            Reset();


            var body = new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            };

            var response = await Register(body);

            string responseBodyString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            AuthResponseDto responseObject = JsonConvert.DeserializeObject<AuthResponseDto>(responseBodyString);

            Assert.NotNull(await userManager.FindByEmailAsync("samuele@girgenti.com"));

        }

        [Fact]
        public async Task Register_CheckingDefaultRoleOnRegistration_AssignedCorrectRoles()
        {
            Reset();

            await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            await Register(new CreateUserRequestDto
            {
                Email = "john@doe.com",
                FirstName = "name",
                LastName = "surname",
                Username = "JohnDoe",
                Password = "Qwe12345@"
            });

            var user1 = await userManager.FindByEmailAsync("samuele@girgenti.com");
            var user2 = await userManager.FindByEmailAsync("john@doe.com");

            Assert.True(await userManager.IsInRoleAsync(user1, "Superadmin"));
            Assert.False(await userManager.IsInRoleAsync(user2, "Superadmin"));

        }

        [Fact]
        public async Task Login_PassingValidFields_UserLoggedIn()
        {
            Reset();

            await Register_PassingValidFields_UserCreated();

            var body = new UserLoginRequestDto
            {
                Email = "samuele@girgenti.com",
                Password = "Qwe12345@"
            };

            var response = await Login(body);
            response.EnsureSuccessStatusCode();

            string responseBodyString = await response.Content.ReadAsStringAsync();
            AuthResponseDto responseObject = JsonConvert.DeserializeObject<AuthResponseDto>(responseBodyString);
        }

        [Fact]
        public async Task Login_PassingUnknownCredentials_UserNotFound()
        {
            Reset();

            var body = new UserLoginRequestDto
            {
                Email = "samuele@girgenti.com",
                Password = "Qwe12345@"
            };

            var response = await Login(body);

            Assert.True(response.StatusCode != HttpStatusCode.OK);
        }


        [Fact]
        public async Task Register_RegisterDuplicateUser_UserRegisteredOnce()
        {
            Reset();

            // Valid user
            var response1 = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            // Username already exists
            var response2 = await Register(new CreateUserRequestDto
            {
                Email = "john@doe.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            // Email already exists
            var response3 = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "JohnDoe",
                Password = "Qwe12345@"
            });

            Assert.True(response1.StatusCode == HttpStatusCode.OK);
            Assert.True(response2.StatusCode != HttpStatusCode.OK);
            Assert.True(response3.StatusCode != HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_RequestingProtectedRouteWithAuthorization_UserAuthorized()
        {
            Reset();

            var registerResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            string responseBodyString = await registerResponse.Content.ReadAsStringAsync();
            AuthResponseDto responseObject = JsonConvert.DeserializeObject<AuthResponseDto>(responseBodyString);

            // Set bearer token
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", responseObject.Token);

            // GET request on protected route
            var protectedRouteResonse = await client.GetAsync($"{UriPrefix}/profiles");

            Assert.True(protectedRouteResonse.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_RequestingProtectedRouteWithoutAuthorization_UserUnauthorized()
        {
            Reset();

            // GET request on protected route without authorization
            var protectedRouteResonse = await client.GetAsync($"{UriPrefix}/users");

            Assert.True(protectedRouteResonse.StatusCode != HttpStatusCode.OK);
        }

    }
}
