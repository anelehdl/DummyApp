using Core.Models;
using MongoDB.Driver;

namespace Infrastructure.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDBSettings _settings;

        public MongoDBContext(MongoDBSettings settings)
        {
            _settings = settings;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoCollection<Staff> StaffCollection =>
            _database.GetCollection<Staff>(_settings.CollectionNames.Staff);

        public IMongoCollection<Role> RolesCollection =>
            _database.GetCollection<Role>(_settings.CollectionNames.Roles);

        public IMongoCollection<Authentication> AuthenticationCollection =>
            _database.GetCollection<Authentication>(_settings.CollectionNames.Authentication);
    }
}
