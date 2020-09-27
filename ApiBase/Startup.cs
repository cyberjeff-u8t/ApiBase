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
