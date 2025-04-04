using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KhachSan.Models
{
    public class CustomerDiscount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonElement("customer_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string CustomerId { get; set; }

        [BsonElement("discount_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string DiscountId { get; set; }

        [BsonElement("is_used")]
        public required bool IsUsed { get; set; }

        [BsonElement("received_at")]
        public required DateTime ReceivedAt { get; set; }

        [BsonElement("used_at")]
        [BsonIgnoreIfNull]
        public DateTime? UsedAt { get; set; } = null;
    }
}
