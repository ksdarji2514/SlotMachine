using MongoDB.Driver;
using SlotMachine.Common.Constants;
using SlotMachine.Database.Entities;
using SlotMachine.Database.Mongo;
using SlotMachine.Repository.Interfaces;

namespace SlotMachine.Repository.Repositories;

public class PlayerRepository(IMongoContext context) : IPlayerRepository
{
    private readonly IMongoCollection<Player> _players = context.GetCollection<Player>(MongoConstants.PlayersCollection);

    public async Task<Player?> TryDeductBetAsync(string playerId, long bet)
    {
        var filter = Builders<Player>.Filter.And(
            Builders<Player>.Filter.Eq(p => p.Id, playerId),
            Builders<Player>.Filter.Gte(p => p.Balance, bet));

        var update = Builders<Player>.Update.Inc(p => p.Balance, -bet);

        return await _players.FindOneAndUpdateAsync(filter, update,
            new FindOneAndUpdateOptions<Player> { ReturnDocument = ReturnDocument.After });
    }

    public async Task<Player> CreditWinAsync(string playerId, long win)
    {
        return await _players.FindOneAndUpdateAsync(
            Builders<Player>.Filter.Eq(p => p.Id, playerId),
            Builders<Player>.Update.Inc(p => p.Balance, win),
            new FindOneAndUpdateOptions<Player> { ReturnDocument = ReturnDocument.After });
    }

    public async Task<Player> AddFundsAsync(string playerId, long amount)
    {
        return await _players.FindOneAndUpdateAsync(
            Builders<Player>.Filter.Eq(p => p.Id, playerId),
            Builders<Player>.Update.Inc(p => p.Balance, amount).SetOnInsert(p => p.Id, playerId),
            new FindOneAndUpdateOptions<Player>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            });
    }

    public async Task<Player?> GetAsync(string playerId) =>
        await _players.Find(Builders<Player>.Filter.Eq(p => p.Id, playerId)).FirstOrDefaultAsync();
}

