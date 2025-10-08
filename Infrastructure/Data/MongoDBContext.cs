using Core.Models;
using MongoDB.Driver;

namespace Infrastructure.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoDatabase _forecastDatabase;
        private readonly MongoDBSettings _settings;

        public MongoDBContext(MongoDBSettings settings)
        {
            _settings = settings;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
            _forecastDatabase = client.GetDatabase(_settings.ForecastDatabaseName);
        }

        // UserDB Collections
        public IMongoCollection<Staff> StaffCollection =>
            _database.GetCollection<Staff>(_settings.CollectionNames.Staff);

        public IMongoCollection<Role> RolesCollection =>
            _database.GetCollection<Role>(_settings.CollectionNames.Roles);

        public IMongoCollection<Authentication> AuthenticationCollection =>
            _database.GetCollection<Authentication>(_settings.CollectionNames.Authentication);

        public IMongoCollection<Client> ClientCollection =>
            _database.GetCollection<Client>(_settings.CollectionNames.Client);



        // ForecastDB Collections
        public IMongoCollection<Inventory> InventoryCollection =>
            _forecastDatabase.GetCollection<Inventory>(_settings.ForecastCollectionNames.Inventory);


        //FOR LATER COLLECTION ADDITIONS (Need to add to models in core)
        //public IMongoCollection<Core.Models.BatchScan> BatchScansCollection =>
        //    _forecastDatabase.GetCollection<Core.Models.BatchScan>(_settings.ForecastCollectionNames.BatchScans);

        //public IMongoCollection<Core.Models.ForecastCache> ForecastCacheCollection =>
        //    _forecastDatabase.GetCollection<Core.Models.ForecastCache>(_settings.ForecastCollectionNames.ForecastCache);

        //public IMongoCollection<Core.Models.Prediction> PredictionCollection =>
        //    _forecastDatabase.GetCollection<Core.Models.Prediction>(_settings.ForecastCollectionNames.Prediction);
    }
}