using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lithium.Web.Infrastructure.Data.Models;

public sealed class DiscordUser
{
    [BsonElement("id"), BsonRepresentation(BsonType.Int64)]
    public required ulong Id { get; init; }
    
    [BsonElement("username"), StringLength(32)]
    public required string Username { get; set; }

    [BsonElement("email"), StringLength(320)]
    public string? Email { get; set; }

    [BsonElement("avatar_url"), StringLength(128)]
    public string? AvatarUrl { get; set; }
}