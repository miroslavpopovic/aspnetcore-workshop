using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace TimeTracker.Client.Security
{
    public static class SecurityExtensions
    {
        public static void AddTokenAuthenticationStateProvider(this IServiceCollection services)
        {
            // Make the same instance accessible as both AuthenticationStateProvider and TokenAuthenticationStateProvider
            services.AddScoped<TokenAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(
                provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());
        }
    }
}
