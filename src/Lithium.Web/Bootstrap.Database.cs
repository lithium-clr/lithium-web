using Lithium.Web.Infrastructure.Data;
using Lithium.Web.Infrastructure.Data.Collections;
using MongoDB.Driver;

namespace Lithium.Web;

public static partial class Bootstrap
{
    private static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connectionString =
            config["Mongo:Uri"] ??
            Environment.GetEnvironmentVariable("MONGO_URI");

        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        var client = new MongoClient(connectionString);
        services.AddMongoDB<WebDbContext>(client, "web");
        services.AddScoped<UserCollection>();

        return services;
    }
}