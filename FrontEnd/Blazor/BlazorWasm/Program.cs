using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Token store (could be SessionStorage-backed in real app)
builder.Services.AddSingleton<ITokenStore, MemoryTokenStore>();

// DelegatingHandler for auth
builder.Services.AddTransient<AuthHandler>();

// Typed HttpClient
builder.Services.AddHttpClient("Api", c =>
{
    c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    c.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<AuthHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

// Lazy assembly loader for on-demand feature modules
builder.Services.AddScoped<Microsoft.AspNetCore.Components.WebAssembly.Services.LazyAssemblyLoader>();

await builder.Build().RunAsync();
