using Lithium.Web.Models;

namespace Lithium.Web.Collections;

public sealed class UserCollection(WebDbContext dbFactory)
    : DbRepository<WebDbContext, User>(dbFactory);