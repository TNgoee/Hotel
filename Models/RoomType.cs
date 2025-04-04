using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KhachSan.Models
{
    public class RoomType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonElement("room_type")]
        public required string RoomTypeName { get; set; }

        [BsonElement("img")]
        public required string Img { get; set; }

        [BsonElement("description")]
        public required string Description { get; set; }

        [BsonElement("price")]
        public required decimal Price { get; set; }
        [BsonElement("room_count")]
        [BsonDefaultValue(0)]
        public int RoomCount { get; set; } = 0;
        [BsonElement("services")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required List<string> ServiceIds { get; set; } = new();
    }
}
