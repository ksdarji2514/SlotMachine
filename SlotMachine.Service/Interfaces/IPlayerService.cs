using SlotMachine.DTO.Requests;
using SlotMachine.DTO.Responses;

namespace SlotMachine.Service.Interfaces;

public interface IPlayerService
{
    Task<UpdateBalanceResponse> UpdateBalanceAsync(UpdateBalanceRequest request);
}
