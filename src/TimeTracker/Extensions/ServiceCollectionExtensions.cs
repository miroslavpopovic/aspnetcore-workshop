using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
    }
}
