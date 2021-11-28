using BlogedWebapp;
using IntegrationTestsProject;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class AccountsControllerTests : IClassFixture<TestingWebAppFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AccountsControllerTests(TestingWebAppFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Read_GET_Action()
        {

            var body = new
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            };

            // Act
            var response = await _client.PostAsync("/api/v1/accounts/register",
                new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
            string responseBodyString = await response.Content.ReadAsStringAsync();
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            //Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
            //Assert.Contains("<h1 class=\"bg-info text-white\">Records</h1>", responseString);
            Assert.True(true);
        }
    }
}
