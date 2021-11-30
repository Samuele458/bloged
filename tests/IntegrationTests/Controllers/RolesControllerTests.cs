using BlogedWebapp;
using BlogedWebapp.Models.Dtos.Generic;
using BlogedWebapp.Models.Dtos.Requests;
using BlogedWebapp.Models.Dtos.Responses;
using IntegrationTestsProject;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class RolesControllerTests : AthenticatorIntegrationTest
    {
        public RolesControllerTests(TestingWebAppFactory<Startup> factory)
            : base(factory)
        {

        }

        [Fact]
        public async void GetAllRoles_ChecksDefaultRoles_DefaultRolesExist()
        {
            Reset();

            // Creating user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            // Set bearer token
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                JsonConvert.DeserializeObject<AuthResponseDto>(await authResponse.Content.ReadAsStringAsync()).Token);

            var res = await client.GetAsync($"{UriPrefix}/roles");
            var roles = JsonConvert.DeserializeObject<string[]>(await res.Content.ReadAsStringAsync());

            Assert.True(res.StatusCode == HttpStatusCode.OK);
            Assert.True(roles.FirstOrDefault(r => r.Equals("Admin")).Equals("Admin"));
            Assert.True(roles.FirstOrDefault(r => r.Equals("Superadmin")).Equals("Superadmin"));
            Assert.True(roles.Length == 2);
        }

        [Fact]
        public async void GetAllRoles_RequestsRolesWithoutAuthorization_Unhautorized()
        {
            Reset();

            var res = await client.GetAsync($"{UriPrefix}/roles");

            Assert.True(res.StatusCode != HttpStatusCode.OK);

        }

        [Fact]
        public async void CreateRole_TryingToCreateRoleWithoutAuthorization_Unhauthorized()
        {
            Reset();

            var createRoleResponse = await client.PostAsync($"{UriPrefix}/roles",
                new StringContent(JsonConvert.SerializeObject(
                    new RoleDto { RoleName = "RoleTest" }), Encoding.UTF8, "application/json"));

            Assert.True(createRoleResponse.StatusCode != HttpStatusCode.OK);
        }

        [Fact]
        public async void CreateRole_TryingToCreateRoleWithAuthorization_RoleCreated()
        {
            Reset();

            // Creating user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(authResponse);

            var createRoleResponse = await client.PostAsync($"{UriPrefix}/roles",
                new StringContent(JsonConvert.SerializeObject(
                    new RoleDto { RoleName = "RoleTest" }), Encoding.UTF8, "application/json"));

            Assert.True(createRoleResponse.StatusCode == HttpStatusCode.OK);
            Assert.True(await roleManager.RoleExistsAsync("RoleTest"));

        }

        [Fact]
        public async void DeleteRole_TryingToDeleteUnknownRole_RoleNotFound()
        {
            Reset();

            // Creating user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(authResponse);

            var res = await client.DeleteAsync($"{UriPrefix}/roles/testrole");

            Assert.True(res.StatusCode != HttpStatusCode.OK);
        }

        [Fact]
        public async void DeleteRole_TryingToDeleteRoleWithoutAuthorization_Unauthorized()
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

            var createRoleResponse = await client.PostAsync($"{UriPrefix}/roles",
                new StringContent(JsonConvert.SerializeObject(
                    new RoleDto { RoleName = "RoleTest" }), Encoding.UTF8, "application/json"));

            // Creating normal user for trying to delete role
            authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            // Sets JWT of normal user (not authorized to delete role)
            await SetJwtBearer(authResponse);

            var res = await client.DeleteAsync($"{UriPrefix}/roles/RoleTest");

            Assert.True(res.StatusCode != HttpStatusCode.OK);
            Assert.True(await roleManager.RoleExistsAsync("RoleTest"));
        }

        [Fact]
        public async void DeleteRole_DeleteRoleWithAuthorization_RoleDeleted()
        {
            Reset();

            // Creating user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(authResponse);

            var createRoleResponse = await client.PostAsync($"{UriPrefix}/roles",
                new StringContent(JsonConvert.SerializeObject(
                    new RoleDto { RoleName = "RoleTest" }), Encoding.UTF8, "application/json"));

            Assert.True(await roleManager.RoleExistsAsync("RoleTest"));

            var deleteRoleResponse = await client.DeleteAsync($"{UriPrefix}/roles/RoleTest");

            Assert.True(deleteRoleResponse.StatusCode == HttpStatusCode.OK);
            Assert.False(await roleManager.RoleExistsAsync("RoleTest"));
        }

        [Fact]
        public async void DeleteRole_TryingToDeleteNonDeletableRole_RoleNotDeleted()
        {
            Reset();

            // Creating user
            var authResponse = await Register(new CreateUserRequestDto
            {
                Email = "samuele@girgenti.com",
                FirstName = "name",
                LastName = "surname",
                Username = "Samuele458",
                Password = "Qwe12345@"
            });

            await SetJwtBearer(authResponse);

            var resAdmin = await client.DeleteAsync($"{UriPrefix}/roles/Admin");
            var resSuperadmin = await client.DeleteAsync($"{UriPrefix}/roles/Superadmin");

            Assert.True(resAdmin.StatusCode != HttpStatusCode.OK);
            Assert.True(resSuperadmin.StatusCode != HttpStatusCode.OK);
            Assert.True(await roleManager.RoleExistsAsync("Admin"));
            Assert.True(await roleManager.RoleExistsAsync("Superadmin"));
        }
    }
}
