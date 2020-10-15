using Api.Base;
using Api.Configuration;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using NSwag;
using System.Linq;
using NSwag.Generation.Processors.Security;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<BookstoreDatabaseSettings>(
                Configuration.GetSection(nameof(BookstoreDatabaseSettings)));

            services.AddSingleton<BookstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);



            services.Configure<FilmstoreDatabaseSettings>(
                Configuration.GetSection(nameof(FilmstoreDatabaseSettings)));

            services.AddSingleton<FilmstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<FilmstoreDatabaseSettings>>().Value);


           
            services.AddSingleton<BaseService<Book>, BookService>();
            services.AddSingleton<BaseService<Film>, FilmService>();

            services.AddControllers();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>
            {
                o.Authority = "http://localhost:7000/auth/realms/cyberRealm";// Configuration["Jwt:Authority"];
                o.Audience = "app-vue";// Configuration["Jwt:Audience"];
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters.ValidateAudience = false;
                o.TokenValidationParameters.ValidateIssuer = false;
                
                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();

                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";

                        //if (env.IsDevelopment())
                        //{
                            return c.Response.WriteAsync(c.Exception.ToString());
                        //}

                        //return c.Response.WriteAsync("An error occured processing your authentication.");
                    },

                    OnForbidden = c =>
                    {
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync("Not authorized");
                    }
                    
                };
            });

            services.AddAuthorization(options =>
            {

                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
              JwtBearerDefaults.AuthenticationScheme);

                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

                options.AddPolicy("test", policy => policy.RequireClaim("roles", "[test]"));
                //options.AddPolicy("test", policy => policy.RequireRole("test"));
            });


            IdentityModelEventSource.ShowPII = true;

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new HeaderApiVersionReader("api-version");

            });

           
            services.AddOpenApiDocument(config =>
              {
                  config.OperationProcessors.Add(new VersionHeader());

                  config.DocumentName = "v1";
                  
                  config.Title = "API Title";
                  config.Version = "1.2.4";
                  config.Description = "API used for xxx";
                  
                  config.ApiGroupNames = new[] { "1.0" };

                  config.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
                  {
                       Type = OpenApiSecuritySchemeType.ApiKey,
                       Name = "Authorization",
                       In = OpenApiSecurityApiKeyLocation.Header,
                       Description = "Type into the textbox: Bearer {your JWT token}."
                   });

                  config.OperationProcessors.Add(
                      new AspNetCoreOperationSecurityScopeProcessor("JWT"));
              
                  config.PostProcess = document =>
                  {
                      document.Info.TermsOfService = "http://zzzz";
                      document.Info.Contact = new NSwag.OpenApiContact
                      {
                          Name = "Jeff Diademi",
                          Email = "jeff.diademi@gmail.com",
                          Url = "http://cyberhom"
                      };
                  };

              });

            

            services.AddOpenApiDocument(config =>
            {
                config.OperationProcessors.Add(new VersionHeader());

                config.DocumentName = "v2";
                config.Title = "API Title";
                config.Version = "2.0.0";

                config.ApiGroupNames = new[] { "1.0", "2.0" };

            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/error");
            
            app.UseHttpsRedirection();

            app.UseRouting();

            
            app.UseAuthorization();

            app.UseAuthentication();

            app.UseOpenApi();
            
            app.UseSwaggerUi3();

            app.UseReDoc(options =>
            {
                options.Path = "/redoc_v1";
                options.DocumentPath = "/swagger/v1/swagger.json";
            });

            app.UseReDoc(options =>
            {
                options.Path = "/redoc_v2";
                options.DocumentPath = "/swagger/v2/swagger.json";
            });         

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
