using Lithium.Web;
using Lithium.Web.Collections;
using Microsoft.AspNetCore.Identity;
using Lithium.Web.Components;
using MongoDB.Driver;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;

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

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Data Protection - IMPORTANT pour Cloudflare
var dataProtectionPath = Path.Combine("/home/app/.aspnet/DataProtection-Keys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("LithiumWeb");

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
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers();

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

// app.UseStaticFiles();
app.UseAntiforgery();

var supportedCultures = new[] { "en-US", "fr-FR" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();