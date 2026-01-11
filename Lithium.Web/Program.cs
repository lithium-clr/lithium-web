using Lithium.Web;
using Lithium.Web.Collections;
using Lithium.Web.Components;
using MongoDB.Driver;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString =
    builder.Configuration["Mongo:Uri"] ??
    Environment.GetEnvironmentVariable("MONGO_URI");

ArgumentException.ThrowIfNullOrEmpty(connectionString);

var client = new MongoClient(connectionString);
builder.Services.AddMongoDB<WebDbContext>(client, "web");
builder.Services.AddScoped<UserCollection>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var dataProtectionPath = Path.Combine("/home/app/.aspnet/DataProtection-Keys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("LithiumWeb");

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "Discord";
    })
    .AddCookie()
    .AddCookie("External")
    .AddDiscord(options =>
    {
        options.SignInScheme = "External";

        options.ClientId = builder.Environment.IsDevelopment()
            ? builder.Configuration["Discord:ClientId"]!
            : Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")!;

        options.ClientSecret = builder.Environment.IsDevelopment() 
            ? builder.Configuration["Discord:ClientSecret"]! 
            : Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")!;
        
        options.Scope.Add("identify");
        options.Scope.Add("email");
        options.SaveTokens = true;

        // Map claims to ensure we get the avatar
        options.ClaimActions.MapJsonKey("urn:discord:avatar:url", "avatar");
    });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();
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