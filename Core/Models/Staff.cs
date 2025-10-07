using MongoDB.Bson;

namespace Core.Models
{
    public class Staff
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("first_name")]
        public string FirstName { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("email")]
        public string Email { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("phone")]
        public string Phone { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("role_id")]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(BsonType.ObjectId)]
        public ObjectId RoleId { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("auth_id")]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(BsonType.ObjectId)]
        public ObjectId AuthId { get; set; }
    }
}
