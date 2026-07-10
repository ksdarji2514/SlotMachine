using SlotMachine.DTO.Requests;
using SlotMachine.DTO.Responses;

namespace SlotMachine.Service.Interfaces;

public interface ISlotService
{
    Task<SpinResponse> SpinAsync(SpinRequest request);

}
