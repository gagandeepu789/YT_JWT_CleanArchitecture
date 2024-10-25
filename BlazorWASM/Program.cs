using Application.Services.Authentication;
using Blazored.LocalStorage;
using BlazorWASM;
using BlazorWASM.States;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7267") });
builder.Services.AddScoped<IAccount, AccountService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAunthenticationStateProvider>();
await builder.Build().RunAsync();
