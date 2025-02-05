using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.RepositoryLayer.Entities
{
    public class OrderItems
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid _id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid ProductID { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal UnitPrice { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Quantity { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TotalPrice { get; set; }
    }
}
