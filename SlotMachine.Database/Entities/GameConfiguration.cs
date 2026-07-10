using MongoDB.Bson.Serialization.Attributes;
using SlotMachine.Common.Constants;

namespace SlotMachine.Database.Entities;

public class GameConfiguration
{
    [BsonId]
    public string Id { get; set; } = MongoConstants.DefaultConfigId;

    [BsonElement("width")]
    public int Width { get; set; }

    [BsonElement("height")]
    public int Height { get; set; }
}
