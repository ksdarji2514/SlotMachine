using SlotMachine.Database.Entities;

namespace SlotMachine.Repository.Interfaces;

public interface IConfigurationRepository
{
    Task<GameConfiguration?> GetAsync();

}
