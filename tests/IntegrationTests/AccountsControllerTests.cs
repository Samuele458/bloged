using BlogedWebapp;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using IntegrationTestsProject;
using Newtonsoft.Json;
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

            //Assert.NotNull(await userManager.FindByEmailAsync("samuele@girgenti.com"));

            Assert.True(true);
        }
    }
}
