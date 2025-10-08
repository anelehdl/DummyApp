using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("sku")]
        public string Sku { get; set; }

        [BsonElement("sku_description")]
        public string SkuDescription { get; set; }

        [BsonElement("user_code")]
        public string UserCode { get; set; }

        [BsonElement("order_date")]
        public DateTime OrderDate { get; set; }

        [BsonElement("previous_order_date")]
        public DateTime? PreviousOrderDate { get; set; }

        [BsonElement("litres")]
        public int Litres { get; set; }

        [BsonElement("days_between_orders")]
        public int DaysBetweenOrders { get; set; }

        [BsonElement("average_daily_use")]
        public double AverageDailyUse { get; set; }

        [BsonElement("user_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId UserId { get; set; }
    }
}