using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KhachSan.Models
{
    public class Discount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonElement("name_discount")]
        public required string NameDiscount { get; set; }

        [BsonElement("price")]
        public required decimal Price { get; set; }

        [BsonElement("description")]
        [BsonIgnoreIfNull]
        public required string Description { get; set; }

        [BsonElement("img")]
        [BsonIgnoreIfNull]
        public string? Img { get; set; }


        [BsonElement("min_amount")]
        public required decimal MinAmount { get; set; }

        [BsonElement("max_quantity")]
        public required int MaxQuantity { get; set; }

        [BsonElement("used_quantity")]
        [BsonDefaultValue(0)]
        public int UsedQuantity { get; set; } = 0;

        [BsonElement("start_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public required DateTime StartDate { get; set; }

        [BsonElement("end_date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public required DateTime EndDate { get; set; }
    }
}
