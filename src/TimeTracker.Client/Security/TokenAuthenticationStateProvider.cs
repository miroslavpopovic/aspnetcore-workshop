using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TimeTracker.Client.Models;

namespace TimeTracker.Client.Security
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public TokenAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();
            var user = await GetUserAsync();

            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(token, user), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task<string> GetTokenAsync() =>
            await _jsRuntime.InvokeAsync<string>("blazorLocalStorage.get", "authToken");

        public async Task<UserModel> GetUserAsync() =>
            await _jsRuntime.InvokeAsync<UserModel>("blazorLocalStorage.get", "user");

        public async Task SetTokenAndUserAsync(string token, UserModel user)
        {
            await _jsRuntime.InvokeAsync<object>("blazorLocalStorage.set", "authToken", token);
            await _jsRuntime.InvokeAsync<object>("blazorLocalStorage.set", "user", user);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt, UserModel user)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            // We need this claim to fill AuthState.User.Identity.Name (to display current user name)
            keyValuePairs.Add("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Name);

            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
