using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace BlogedWebapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogedWebapp", Version = "v1" });
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogedWebapp v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });

            const string dashboardPath = "/dashboard";
            const string blogPath = "";
            if (env.IsDevelopment())
            {
                app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments(dashboardPath)
                                   || ctx.Request.Path.StartsWithSegments("/sockjs-node"),
                    client =>
                    {
                        client.UseSpa(spa =>
                        {
                            spa.Options.SourcePath = "Client/packages/dashboard";
                            spa.UseReactDevelopmentServer("start-from-aspnet");
                            //spa.UseProxyToSpaDevelopmentServer("http://localhost:3006");
                        });
                    });

                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "Client/packages/blog";
                    spa.UseReactDevelopmentServer("start-from-aspnet");
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:3006");
                });


            }



            /*app.Map("/dashboard",
               adminApp =>
               {
                   adminApp.UseSpa(spa =>
                   {
                       spa.Options.SourcePath = "Client/packages/dashboard/build";
                       spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                       {
                           FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Client", "packages", "dashboard", "build"))
                       };

                   });
               });*/

            app.UseStaticFiles();

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



            /*
            else
            {
                app.Map(new PathString(dashboardPath), client =>
                {
                    // `https://github.com/dotnet/aspnetcore/issues/3147`
                    client.UseSpaStaticFiles(new StaticFileOptions()
                    {
                        OnPrepareResponse = ctx =>
                        {
                            if (ctx.Context.Request.Path.StartsWithSegments($"{dashboardPath}/static"))
                            {
                                // Cache all static resources for 1 year (versioned file names)
                                var headers = ctx.Context.Response.GetTypedHeaders();
                                headers.CacheControl = new CacheControlHeaderValue
                                {
                                    Public = true,
                                    MaxAge = System.TimeSpan.FromDays(365)
                                };
                            }
                            else
                            {
                                // Do not cache explicit `/index.html` or any other files.  See also: `DefaultPageStaticFileOptions` below for implicit "/index.html"
                                var headers = ctx.Context.Response.GetTypedHeaders();
                                headers.CacheControl = new CacheControlHeaderValue
                                {
                                    Public = true,
                                    MaxAge = TimeSpan.FromDays(0)
                                };
                            }
                        }
                    });

                    client.UseSpa(spa =>
                    {

                        spa.Options.SourcePath = "Client/packages/dashboard/build";
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
                            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Client", "packages", "dashboard", "build"))
                        };
                    });
                });


                app.Map(new PathString(blogPath), client =>
                {
                    // `https://github.com/dotnet/aspnetcore/issues/3147`
                    client.UseSpaStaticFiles(new StaticFileOptions()
                    {
                        OnPrepareResponse = ctx =>
                        {
                            if (ctx.Context.Request.Path.StartsWithSegments($"/static"))
                            {
                                // Cache all static resources for 1 year (versioned file names)
                                var headers = ctx.Context.Response.GetTypedHeaders();
                                headers.CacheControl = new CacheControlHeaderValue
                                {
                                    Public = true,
                                    MaxAge = System.TimeSpan.FromDays(365)
                                };
                            }
                            else
                            {
                                // Do not cache explicit `/index.html` or any other files.  See also: `DefaultPageStaticFileOptions` below for implicit "/index.html"
                                var headers = ctx.Context.Response.GetTypedHeaders();
                                headers.CacheControl = new CacheControlHeaderValue
                                {
                                    Public = true,
                                    MaxAge = TimeSpan.FromDays(0)
                                };
                            }
                        }
                    });

                    client.UseSpa(spa =>
                    {

                        spa.Options.SourcePath = "Client/packages/blog/build";
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
                            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Client", "packages", "blog", "build"))
                        };
                    });
                });
            }*/
        }
    }
}
