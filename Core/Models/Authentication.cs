using MongoDB.Bson;

namespace Core.Models
{
    public class Authentication
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("authID")]
        public string AuthID { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("salt")]
        public string Salt { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("hashed_password")]
        public string HashedPassword { get; set; }
    }
}
