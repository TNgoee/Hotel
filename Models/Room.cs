using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KhachSan.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name_room")]
        public required string NameRoom { get; set; }

        [BsonElement("floor")]
        public required int Floor { get; set; }

        [BsonElement("img")]
        [BsonIgnoreIfNull]
        public string? Img { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public required RoomStatus Status { get; set; }

        [BsonElement("room_type_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string RoomTypeId { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public enum RoomStatus
    {
        Available,
        NotAvailable,

    }
}
