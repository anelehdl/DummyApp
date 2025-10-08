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
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(_settings.ConnectionString))
                throw new ArgumentException("MongoDB ConnectionString cannot be null or empty");

            if (string.IsNullOrEmpty(_settings.DatabaseName))
                throw new ArgumentException("MongoDB DatabaseName cannot be null or empty");

            if (string.IsNullOrEmpty(_settings.ForecastDatabaseName))
                throw new ArgumentException("MongoDB ForecastDatabaseName cannot be null or empty. Check appsettings.json");

            if (_settings.ForecastCollectionNames == null)
                throw new ArgumentException("ForecastCollectionNames configuration is missing. Check appsettings.json");

            if (string.IsNullOrEmpty(_settings.ForecastCollectionNames.Inventory))
                throw new ArgumentException("ForecastCollectionNames.Inventory is not configured. Check appsettings.json");

            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);

            // Connect to ForecastDB for inventory data
            _forecastDatabase = client.GetDatabase(_settings.ForecastDatabaseName);

            Console.WriteLine($"MongoDBContext initialized:");
            Console.WriteLine($"  - UserDB: {_settings.DatabaseName}");
            Console.WriteLine($"  - ForecastDB: {_settings.ForecastDatabaseName}");
            Console.WriteLine($"  - Inventory Collection: {_settings.ForecastCollectionNames.Inventory}");
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



        //public IMongoCollection<Core.Models.BatchScan> BatchScansCollection =>
        //    _forecastDatabase.GetCollection<Core.Models.BatchScan>(_settings.ForecastCollectionNames.BatchScans);

        //public IMongoCollection<Core.Models.ForecastCache> ForecastCacheCollection =>
        //    _forecastDatabase.GetCollection<Core.Models.ForecastCache>(_settings.ForecastCollectionNames.ForecastCache);

        //public IMongoCollection<Core.Models.Prediction> PredictionCollection =>
        //    _forecastDatabase.GetCollection<Core.Models.Prediction>(_settings.ForecastCollectionNames.Prediction);
    }
}