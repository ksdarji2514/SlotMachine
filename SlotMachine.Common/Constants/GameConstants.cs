namespace SlotMachine.Common.Constants;

public static class GameConstants
{
    public const int MinDigit = 0;
    public const int MaxDigitExclusive = 10;    // Random upper bound is exclusive => 0-9
    public const int MinWinningSeriesLength = 3; // a paying run must be longer than 2
}
