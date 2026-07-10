using SlotMachine.Database.Entities;

namespace SlotMachine.Repository.Interfaces;

public interface IPlayerRepository
{
    /// <summary>Atomic check-and-deduct. Null if player missing or funds too low.</summary>
    Task<Player?> TryDeductBetAsync(string playerId, long bet);

    /// <summary>Atomically add winnings. Returns the updated player.</summary>
    Task<Player> CreditWinAsync(string playerId, long win);

    /// <summary>Atomically add funds; upserts (creates the player if new).</summary>
    Task<Player> AddFundsAsync(string playerId, long amount);

    /// <summary>Read a player, or null if not found.</summary>
    Task<Player?> GetAsync(string playerId);
}
