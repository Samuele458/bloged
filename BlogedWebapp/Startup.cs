using BlogedWebapp.Authorizations;
using BlogedWebapp.Authorizations.Handlers;
using BlogedWebapp.Data;
using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlogedWebapp
{

    public static class MvcOptionsExtensions
    {
        public static void UseGeneralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Add(new RoutePrefixConvention(routeAttribute));
        }

        public static void UseGeneralRoutePrefix(this MvcOptions opts, string
        prefix)
        {
            opts.UseGeneralRoutePrefix(new RouteAttribute(prefix));
        }
    }

    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _routePrefix;

        public RoutePrefixConvention(IRouteTemplateProvider route)
        {
            _routePrefix = new AttributeRouteModel(route);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
                }
                else
                {
                    selector.AttributeRouteModel = _routePrefix;
                }
            }
        }
    }

    /*public class SecurityRequirementsOperationFilter
    {
        public SecurityRequirementsOperationFilter(bool includeUnauthorizedAndForbiddenResponses = true, string securitySchemaName = "oauth2")
    {
        Func<IEnumerable<AuthorizeAttribute>, IEnumerable<string>> policySelector = (IEnumerable<AuthorizeAttribute> authAttributes) => authAttributes.Where((Func<AuthorizeAttribute, bool>)((AuthorizeAttribute a) => !string.IsNullOrEmpty(a.Policy))).Select((Func<AuthorizeAttribute, string>)((AuthorizeAttribute a) => a.Policy));
        filter = new SecurityRequirementsOperationFilter<AuthorizeAttribute>(policySelector, includeUnauthorizedAndForbiddenResponses, securitySchemaName);
    }
}*/

    public class Startup
    {
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            System.Diagnostics.Debug.WriteLine("Current environment: " + env.EnvironmentName);
            CurrentEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Environment.GetEnvironmentVariable("BLOGED_DB_CONN")));

            AppSettings currentSettings = new AppSettings();

            // In production reads config from AWS SecretsManager
            if (CurrentEnvironment.EnvironmentName == "Production")
            {
                JObject secrets = null;

                string secretsString = SecretsManager.GetSecret("prod/Bloged/secrets");

                secrets = JObject.Parse(secretsString);

                currentSettings.JwtSecret = secrets["JwtSecret"].ToString();
                currentSettings.ConnectionString = secrets["ConnectionString"].ToString();
            }

            // In development reads config from appsettings files
            if (CurrentEnvironment.EnvironmentName == "Development")
            {
                currentSettings.JwtSecret = Configuration
                                                .GetSection("JwtSettings")
                                                .GetValue<string>("JwtSecret");

                currentSettings.ConnectionString = Configuration
                                                        .GetSection("Database")
                                                        .GetValue<string>("ConnectionString");
            }

            currentSettings.JwtExpiryTimeFrame = TimeSpan.Parse(Configuration
                                                                    .GetSection("JwtSettings")
                                                                    .GetValue<string>("ExpiryTimeFrame"));


            services.Configure<AppSettings>((AppSettings) =>
            {
                AppSettings.JwtSecret = currentSettings.JwtSecret;
                AppSettings.JwtExpiryTimeFrame = currentSettings.JwtExpiryTimeFrame;
                AppSettings.ConnectionString = currentSettings.ConnectionString;

            });


            //Creating db connection for Entity Framework Core
            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(currentSettings.ConnectionString));

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });


            //JSON serializer
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                    .Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
                                                            = new DefaultContractResolver());

            //apply "api" prefix on routes
            services.AddControllersWithViews(o =>
            {
                o.UseGeneralRoutePrefix("api");
            });

            //Adding swagger API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogedWebapp", Version = "v1" });

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // Add this filter as well.
                c.OperationFilter<SecurityRequirementsOperationFilter>();

            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            //services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddApiVersioning(opt =>
            {
                //report api versions on response
                opt.ReportApiVersions = true;

                //if not specified, assume default api version
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = ApiVersion.Default;
            });

            //getting jwt secret key
            var key = Encoding.ASCII.GetBytes(currentSettings.JwtSecret);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Injecting token options
            services.AddSingleton(tokenValidationParameters);

            // Adding authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowedToUse", Policies.AllowedToUse());
                options.AddPolicy("AdminOrSuperadmin", Policies.AdminOrSuperadmin());
                options.AddPolicy("AllowedToUseBlog", Policies.AllowedToUseBlog());
            });

            // Adding policy handlers
            services.AddSingleton<IAuthorizationHandler, OwnerAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, BlogRolesAuthorizationHandler>();

            // Adding JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameters;
            });

            // Adding identities
            services
                .AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<DataContext>();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                                IWebHostEnvironment env,
                                IServiceProvider serviceProvider)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogedWebapp v1"));
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });

            //Dashbord SPA path
            const string dashboardPath = "/dashboard";

            if (env.IsDevelopment())
            {

                //map dashboard spa in development
                app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments(dashboardPath)
                                   || ctx.Request.Path.StartsWithSegments("/sockjs-node"),
                    client =>
                    {
                        client.UseSpa(spa =>
                        {
                            //spa.Options.SourcePath = "Client/packages/dashboard";
                            //spa.UseReactDevelopmentServer("start-from-aspnet");
                            spa.UseProxyToSpaDevelopmentServer("http://localhost:3001");
                        });
                    });

                //map blog spa in development
                app.UseSpa(spa =>
                {
                    //spa.Options.SourcePath = "Client/packages/blog";
                    //spa.UseReactDevelopmentServer("start-from-aspnet");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3002");
                });

            }
            else
            {
                //serve static files on wwwroot folder
                app.UseStaticFiles();

                //map dashboard SPA in production
                app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments(dashboardPath)
                                       || ctx.Request.Path.StartsWithSegments("/sockjs-node"),
                client =>
                {
                    client.UseSpa(spa =>
                    {

                        spa.Options.SourcePath = "wwwroot/dashboard";
                        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions()
                        {
                            OnPrepareResponse = ctx =>
                            {
                                // Do not cache implicit `/index.html`.  See also: `UseSpaStaticFiles` above
                                var headers = ctx.Context.Response.GetTypedHeaders();
                                headers.CacheControl = new CacheControlHeaderValue
                                {
                                    Public = true,
                                    MaxAge = TimeSpan.FromDays(0)
                                };
                            }
                        };
                        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                        {
                            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dashboard"))
                        };
                    });
                });

                //map blog spa in production
                app.UseSpa(spa =>
                {

                    spa.Options.SourcePath = "wwwroot";
                    spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions()
                    {
                        OnPrepareResponse = ctx =>
                        {
                            // Do not cache implicit `/index.html`.  See also: `UseSpaStaticFiles` above
                            var headers = ctx.Context.Response.GetTypedHeaders();
                            headers.CacheControl = new CacheControlHeaderValue
                            {
                                Public = true,
                                MaxAge = TimeSpan.FromDays(0)
                            };
                        }
                    };
                    spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                    };
                });
            }


            CreateRoles(serviceProvider);
            //var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
        }

        public static void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            List<String> rolesList = new List<string> { "Superadmin", "Admin" };

            foreach (string roleName in rolesList)
            {
                var roleExists = roleManager.RoleExistsAsync(roleName).Result;
                if (!roleExists)
                {
                    var created = roleManager.CreateAsync(new IdentityRole(roleName)).Result;
                }
            }
        }
    }
}
