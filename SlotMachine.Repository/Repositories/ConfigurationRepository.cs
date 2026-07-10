using MongoDB.Driver;
using SlotMachine.Common.Constants;
using SlotMachine.Database.Entities;
using SlotMachine.Database.Mongo;
using SlotMachine.Repository.Interfaces;

namespace SlotMachine.Repository.Repositories;

public class ConfigurationRepository(IMongoContext context) : IConfigurationRepository
{
    private readonly IMongoCollection<GameConfiguration> _configs = context.GetCollection<GameConfiguration>(MongoConstants.ConfigCollection)                       ;

    // Read fresh each call => matrix size is reconfigurable without a restart.
    public async Task<GameConfiguration?> GetAsync() =>
        await _configs
            .Find(Builders<GameConfiguration>.Filter.Eq(c => c.Id, MongoConstants.DefaultConfigId))
            .FirstOrDefaultAsync();
}