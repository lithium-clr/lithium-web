using Lithium.Web.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Lithium.Web.Infrastructure.Data;

public sealed class WebDbContext(DbContextOptions<WebDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().ToCollection("users");
    }
}