using MongoDB.Bson;

namespace Core.Models
{
    public class Role
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("name")]
        public string Name { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("description")]
        public string Description { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonElement("permissions")]
        public List<string> Permissions { get; set; }
    }
}
