using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;

namespace Lithium.Web;

public static partial class Bootstrap
{
    extension(WebApplicationBuilder builder)
    {
        public void SetupBootstrap()
        {
            builder.Services.AddRazorComponents(options => 
                options.DetailedErrors = builder.Environment.IsDevelopment())
                .AddInteractiveServerComponents();
            
            var dataProtectionPath = Path.Combine("/home/app/.aspnet/DataProtection-Keys");
            
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
                .SetApplicationName("LithiumWeb");

            builder.Services.SetupSentry(builder.Environment, builder.Configuration);
            builder.Services.SetupDatabase(builder.Configuration);
            builder.Services.SetupLocalization();
            builder.Services.SetupAuthentication(builder.Environment, builder.Configuration);
            
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                           ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
            
            builder.Services.AddControllers();
        }
    }
    
    extension(WebApplication app)
    {
        public void SetupBootstrap()
        {
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

            app.SetupLanguages();

            app.MapStaticAssets();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapControllers();
        }
    }
}