using SlotMachine.DTO.Requests;
using SlotMachine.DTO.Responses;
using SlotMachine.Repository.Interfaces;
using SlotMachine.Service.Interfaces;

namespace SlotMachine.Service.Services;

public class PlayerService(IPlayerRepository players) : IPlayerService
{
    private readonly IPlayerRepository _players = players;

    public async Task<UpdateBalanceResponse> UpdateBalanceAsync(UpdateBalanceRequest request)
    {
        var player = await _players.AddFundsAsync(request.PlayerId, request.Amount);

        return new UpdateBalanceResponse
        {
            PlayerId = player.Id,
            Balance = player.Balance
        };
    }
}
