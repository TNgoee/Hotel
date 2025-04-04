using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace KhachSan.Models
{
    public class ReviewService
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonElement("custom_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string CustomId { get; set; }

        [BsonElement("service_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string ServiceId { get; set; }

        [BsonElement("rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public required int Rating { get; set; }

        [BsonElement("comment")]
        [BsonIgnoreIfNull]
        public string? Comment { get; set; }

        [BsonElement("created_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
