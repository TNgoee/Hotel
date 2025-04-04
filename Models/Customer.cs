using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KhachSan.Models
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("birthday")]
        public required DateTime Birthday { get; set; }
        [BsonElement("avt")]
        public string? Avatar { get; set; }
        [BsonElement("account_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
