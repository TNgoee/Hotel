using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KhachSan.Models
{
    public class CategoryRoom
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        public required string NameCategoryRoom { get; set; }
    }
}
