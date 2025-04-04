using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KhachSan.Models
{
    public class Request
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonElement("customer_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string CustomerId { get; set; }

        [BsonElement("room_type_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string RoomTypeId { get; set; }

        [BsonElement("assigned_room_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string? AssignedRoomId { get; set; }

        [BsonElement("date_start")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public required DateTime DateStart { get; set; }

        [BsonElement("date_end")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public required DateTime DateEnd { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public required RequestStatus Status { get; set; }

        [BsonElement("total_amount")]
        public required decimal TotalAmount { get; set; }

        [BsonElement("discount_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string? DiscountId { get; set; }

        [BsonElement("final_amount")]
        public required decimal FinalAmount { get; set; }

        [BsonElement("created_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum RequestStatus
    {
        Pending,
        Confirmed,
        Canceled,
        Completed
    }
}
