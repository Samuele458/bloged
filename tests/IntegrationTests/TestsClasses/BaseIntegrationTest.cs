using BlogedWebapp;
using BlogedWebapp.Helpers;
using IntegrationTestsProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace IntegrationTests
{

    /// <summary>
    ///  Base class for building integration tests
    /// </summary>
    public class BaseIntegrationTest : IClassFixture<TestingWebAppFactory<Startup>>
    {
        protected HttpClient client;

        protected readonly TestingWebAppFactory<Startup> factory;

        // Db context for checking database
        protected DataContext context;

        protected IServiceProvider serviceProvider;

        public BaseIntegrationTest(TestingWebAppFactory<Startup> factory)
        {
            this.factory = factory;
            this.client = factory.CreateClient();
            this.serviceProvider = factory
                                       .Services
                                       .CreateScope()
                                       .ServiceProvider;

            this.context = serviceProvider.GetRequiredService<DataContext>();

        }

        /// <summary>
        ///  Empties a DbSet
        /// </summary>
        /// <typeparam name="T">DbSet entity type</typeparam>
        /// <param name="set">DbSet to empty</param>
        public static void GenericRemoveSet<T>(DbSet<T> set) where T : class
        {
            foreach (var item in set)
            {
                set.Remove(item);
            }
        }

        /// <summary>
        ///  Empties a DbCOntext
        /// </summary>
        /// <param name="context">DbContext to empty</param>
        public static void ClearGenericDbContext(DbContext context)
        {
            var removeMethod = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.GetMethod("GenericRemoveSet");
            foreach (var prop in context.GetType().GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)))
            {
                var typedRemove = removeMethod.MakeGenericMethod(prop.PropertyType.GetGenericArguments().First());
                typedRemove.Invoke(null, new object[] { prop.GetValue(context) });
            }
            context.SaveChanges();
        }

        /// <summary>
        ///  Reset test state
        /// </summary>
        public virtual void Reset()
        {
            ClearGenericDbContext(context);
            Startup.CreateRoles(serviceProvider);

            client.DefaultRequestHeaders.Clear();
        }

        /// <summary>
        ///  ERI prefix containing api prefix and version
        /// </summary>
        public string UriPrefix
        {
            get { return $"/{IntegrationTestSettings.ApiPrefix}/v{IntegrationTestSettings.ApiVersion}"; }
        }

    }
}
