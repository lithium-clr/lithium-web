using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Lithium.Web;

public static partial class Bootstrap
{
    private static IServiceCollection SetupAuthentication(this IServiceCollection services, IHostEnvironment env,
        IConfiguration config)
    {
        services.AddCascadingAuthenticationState();

        services.AddAuthentication(options =>
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

                options.ClientId = env.IsDevelopment()
                    ? config["Discord:ClientId"]!
                    : Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID")!;

                options.ClientSecret = env.IsDevelopment()
                    ? config["Discord:ClientSecret"]!
                    : Environment.GetEnvironmentVariable("DISCORD_CLIENT_SECRET")!;

                options.Scope.Add("identify");
                options.Scope.Add("email");
                options.SaveTokens = true;

                // Map claims to ensure we get the avatar
                options.ClaimActions.MapJsonKey("urn:discord:avatar:url", "avatar");
            });

        return services;
    }
}