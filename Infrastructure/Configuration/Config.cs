public class MongoDBSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public CollectionNames CollectionNames { get; set; }
}

public class CollectionNames
{
    public string Staff { get; set; }
    public string Roles { get; set; }
    public string Authentication { get; set; }
    public string Client { get; set; }
}