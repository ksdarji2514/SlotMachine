namespace SlotMachine.DTO.Requests;

public record SpinRequest
{
    public string PlayerId { get; init; } = default!;
    public long Bet { get; init; }
}
