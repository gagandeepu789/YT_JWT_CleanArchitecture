using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace BlazorWASM.States
{
    public class CustomAunthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        private readonly ILocalStorageService localStorageService;
        private const string LocalStorageKey = "auth";

        public CustomAunthenticationStateProvider(ILocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token=await localStorageService.GetItemAsStringAsync(LocalStorageKey);   
            if (string.IsNullOrEmpty(token)) 
                return await Task.FromResult(new AuthenticationState(anonymous));
            var(name,email)=GetClaims(token);
            if (string.IsNullOrEmpty(token))
                return await Task.FromResult(new AuthenticationState(anonymous));

            var claims= SetClaimPrincipal(name,email);
            if(claims is null )
                return await Task.FromResult(new AuthenticationState(anonymous));
            else
                return await Task.FromResult(new AuthenticationState(claims));

        }
        public static ClaimsPrincipal SetClaimPrincipal(string name,string email)
        {
            if (name is null || email is null) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(
                [
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email)
                ], "JwtAuth"));
        }
       private static (string,string) GetClaims(String jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken)) return(null!, null!);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var name=token.Claims.FirstOrDefault(_=>_.Type==ClaimTypes.Name)?.Value;    
            var email=token.Claims.FirstOrDefault(_=>_.Type==ClaimTypes.Email)?.Value;
            return (name, email);   
        }
        public async Task UpdateAuthenticationState( string jwtToken)
        {
            var claims = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                var(name,email)=GetClaims(jwtToken);    
                if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
                        return;

                var setClaims=SetClaimPrincipal(name,email);
                if(setClaims is null) return;       
            }
            else
            {

                await localStorageService.RemoveItemAsync(LocalStorageKey);
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claims)));
        }
        
       
    }
}
