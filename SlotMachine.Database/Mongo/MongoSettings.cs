namespace SlotMachine.Database.Mongo;

/// <summary>Bound from the "MongoDb" section of appsettings.json.</summary>
public class MongoSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
}
