using MongoDB.Bson;

namespace Core.Models
{
    public class Client
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("user_code")]
        public string UserCode { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("username")]
        public string Username { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("role_id")]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(BsonType.ObjectId)]
        public ObjectId RoleId { get; set; }
    }
}