using System;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimeTracker.Data;
using TimeTracker.Extensions;
using TimeTracker.Models.Validation;

namespace TimeTracker
{
    public class Startup
    {
        public static Action<IConfiguration, DbContextOptionsBuilder> ConfigureDbContext = (configuration, options) =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TimeTrackerDbContext>(options => ConfigureDbContext(Configuration, options));

            services.AddJwtBearerAuthentication(Configuration);

            services.AddCors();

            services.AddControllers().AddFluentValidation(
                fv => fv.RegisterValidatorsFromAssemblyContaining<UserInputModelValidator>());

            services.AddVersioning();

            services.AddOpenApi();

            services.AddHealthChecks()
                .AddSqlite(Configuration.GetConnectionString("DefaultConnection"));

            services.AddHealthChecksUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ErrorHandlingMiddleware>();
            //app.UseMiddleware<LimitingMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // NOTE: this is just for demo purpose! Usually, you should limit access to a specific origin
            // More info: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0
            app.UseCors(
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHealthChecksUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
