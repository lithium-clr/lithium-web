using Sentry.Extensions.Logging;
using Sentry.Extensions.Logging.Extensions.DependencyInjection;

namespace Lithium.Web;

public static partial class Bootstrap
{
    private static IServiceCollection SetupSentry(this IServiceCollection services, IHostEnvironment env,
        IConfiguration config)
    {
        services.Configure<SentryLoggingOptions>(options =>
        {
            options.Dsn = env.IsDevelopment()
                ? config["Sentry:Dsn"]
                : Environment.GetEnvironmentVariable("SENTRY_DSN");
            options.Debug = env.IsDevelopment();
            
            options.AttachStacktrace = true;
            options.Environment = config["Environment"];
            options.MinimumEventLevel = LogLevel.Error;
            options.MinimumBreadcrumbLevel = LogLevel.Debug;
            options.InitializeSdk = true;
            options.SendDefaultPii = true;
            options.EnableLogs = true;
            options.SampleRate = 1f;
        });
        
        services.AddSentry<SentryLoggingOptions>();
        return services;
    }
}