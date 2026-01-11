using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lithium.Web.Infrastructure.Data.Models;

public sealed class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; init; } = Guid.NewGuid();

    [BsonElement("username"), StringLength(32)]
    public required string Username { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("discord_id")]
    public required string DiscordId { get; init; }

    [BsonElement("avatar_url")]
    public string? AvatarUrl { get; set; }
}