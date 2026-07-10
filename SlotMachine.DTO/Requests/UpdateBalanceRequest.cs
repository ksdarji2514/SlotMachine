namespace SlotMachine.DTO.Requests;

public record UpdateBalanceRequest
{
    public string PlayerId { get; init; } = default!;
    public long Amount { get; init; }
}
