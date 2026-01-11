using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lithium.Web.Infrastructure.Data.Models;

public sealed class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; } = Guid.NewGuid();

    [BsonElement("discord")]
    public required DiscordUser Discord { get; init; }

    [BsonElement("roles")]
    public List<string> Roles { get; set; } = [];
}
