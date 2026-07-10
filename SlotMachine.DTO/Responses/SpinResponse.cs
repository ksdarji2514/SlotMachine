namespace SlotMachine.DTO.Responses;

/// <summary>Matrix indexed [row][column]: Height rows, Width cols, each cell 0-9.</summary>
public record SpinResponse
{
    public int[][] Matrix { get; init; } = default!;
    public long Win { get; init; }
    public long Balance { get; init; }
}
