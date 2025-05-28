using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;
using Client;
using Client.Providers;
using Client.Contracts;
using Client.Service;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.IdentityModel.Tokens.Jwt;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;
services.AddOptions();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

services.AddAuthorizationCore();
services.AddScoped<ApiAuthenticationStateProvider>();
services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ApiAuthenticationStateProvider>());
services.AddScoped<JwtSecurityTokenHandler>();

services.AddBlazoredLocalStorage();
services.AddBlazoredSessionStorage();
services.AddBlazoredToast();

services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
services.AddTransient<IArticleRepository, ArticleRepository>();
services.AddTransient<IOrderRepository, OrderRepository>();

await builder.Build().RunAsync();
