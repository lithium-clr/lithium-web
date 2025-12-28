namespace Lithium.Web.Models;

public sealed class UserCollection(WebDbContext dbFactory)
    : DbRepository<WebDbContext, User>(dbFactory);