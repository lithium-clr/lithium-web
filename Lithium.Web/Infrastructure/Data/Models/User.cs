using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lithium.Web.Infrastructure.Data.Models;

public sealed class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; } = Guid.NewGuid();

    [BsonElement("username"), StringLength(64)]
    public required string Username { get; set; }

    [BsonElement("email"), StringLength(320)]
    public string? Email { get; set; }

    [BsonElement("discord_id"), BsonRepresentation(BsonType.Int64)]
    public required ulong DiscordId { get; init; }

    [BsonElement("avatar_url"), StringLength(128)]
    public string? AvatarUrl { get; set; }
}