using Lithium.Web;
using Lithium.Web.Collections;
using Lithium.Web.Models;
using Microsoft.AspNetCore.Identity;
using Lithium.Web.Components;
using MongoDB.Driver;

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

// ⚠️ PAS de HTTPS redirection
// Cloudflare gère TLS, l'app reste en HTTP

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // ❌ pas de HSTS (inutile sans HTTPS local)
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();