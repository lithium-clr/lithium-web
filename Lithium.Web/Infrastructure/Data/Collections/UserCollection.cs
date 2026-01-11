using Lithium.Web.Infrastructure.Data.Models;

namespace Lithium.Web.Infrastructure.Data.Collections;

public sealed class UserCollection(WebDbContext dbFactory)
    : DbRepository<WebDbContext, User>(dbFactory);