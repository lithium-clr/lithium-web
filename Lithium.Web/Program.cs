using Lithium.Web;
using Lithium.Web.Collections;
using Lithium.Web.Models;
using Microsoft.AspNetCore.Identity;
using Lithium.Web.Components;
using MongoDB.Driver;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Razor / Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MongoDB
var connectionString =
    builder.Configuration["Mongo:Uri"] ??
    Environment.GetEnvironmentVariable("MONGO_URI");

ArgumentException.ThrowIfNullOrEmpty(connectionString);

var client = new MongoClient(connectionString);
builder.Services.AddMongoDB<WebDbContext>(client, "web");
builder.Services.AddScoped<UserCollection>();

// Auth
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Configuration pour proxy inverse (Cloudflare)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                               ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// IMPORTANT : Forwarded headers AVANT tout le reste
app.UseForwardedHeaders();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();