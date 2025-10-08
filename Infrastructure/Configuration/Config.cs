public class MongoDBSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string ForecastDatabaseName { get; set; }
    public CollectionNames CollectionNames { get; set; }
    public ForecastCollectionNames ForecastCollectionNames { get; set; }
}

public class CollectionNames
{
    public string Staff { get; set; }
    public string Roles { get; set; }
    public string Authentication { get; set; }
    public string Client { get; set; }
}

public class ForecastCollectionNames
{
    public string Inventory { get; set; }
    public string BatchScans { get; set; }
    public string ForecastCache { get; set; }
    public string Prediction { get; set; }
}