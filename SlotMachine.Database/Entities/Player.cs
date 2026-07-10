using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SlotMachine.Database.Entities;

public class Player
{
    [BsonId]
    public string Id { get; set; } = default!;

    [BsonElement("balance")]
    public long Balance { get; set; }
}
