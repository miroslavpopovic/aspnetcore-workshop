using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace TimeTracker.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        var tokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = configuration["Tokens:Issuer"],
                            ValidAudience = configuration["Tokens:Issuer"],
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(configuration["Tokens:Key"]))
                        };

                        options.TokenValidationParameters = tokenValidationParameters;
                    });
        }

        public static void AddOpenApi(this IServiceCollection services)
        {
            services.AddSwaggerDocument(
                options =>
                {
                    options.OperationProcessors.Add(
                        new OperationSecurityScopeProcessor("jwt-token"));
                    options.DocumentProcessors.Add(
                        new SecurityDefinitionAppender(
                            "jwt-token", new[] { "" }, new OpenApiSecurityScheme
                            {
                                Type = OpenApiSecuritySchemeType.ApiKey,
                                Name = "Authorization",
                                Description =
                                    "Enter \"Bearer jwt-token\" as value. " +
                                    "Use https://localhost:44383/get-token to get read-only JWT token. " +
                                    "Use https://localhost:44383/get-token?admin=true to get admin (read-write) JWT token.",
                                In = OpenApiSecurityApiKeyLocation.Header
                            }));

                    options.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "Time Tracker API v1";
                        document.Info.Description = "An API for ASP.NET Core Workshop";
                        document.Info.TermsOfService = "Do whatever you want with it :)";
                        document.Info.Contact = new OpenApiContact
                        {
                            Name = "Miroslav Popovic",
                            Email = string.Empty,
                            Url = "https://miroslavpopovic.com"
                        };
                        document.Info.License = new OpenApiLicense
                        {
                            Name = "MIT",
                            Url = "https://opensource.org/licenses/MIT"
                        };
                    };
                });
        }
    }
}
