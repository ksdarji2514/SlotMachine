namespace SlotMachine.DTO.Responses;

public record UpdateBalanceResponse
{
    public string PlayerId { get; init; } = default!;
    public long Balance { get; init; }
}