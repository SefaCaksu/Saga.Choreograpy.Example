using MongoDB.Bson.Serialization.Attributes;

namespace Stock.API.Models
{
    public class Stock
	{
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.CSharpLegacy)]
        [BsonElement(Order = 0)]
        public Guid Id { get; set; }

        [BsonElement(Order = 1)]
        public string ProductId { get; set; }

        [BsonElement(Order = 2)]
        [BsonRepresentation(MongoDB.Bson.BsonType.Int64)]
        public int Count { get; set; }

        [BsonElement(Order = 3)]
        [BsonRepresentation(MongoDB.Bson.BsonType.DateTime)]
        public DateTime CreatedDate { get; set; }
    }
}

