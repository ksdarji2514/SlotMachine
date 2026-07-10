using SlotMachine.Common.Constants;
using SlotMachine.Service.Interfaces;

namespace SlotMachine.Service.Services;

/// <summary>
/// Total win for a spin.
///
/// A win line picks one cell per column, read left to right. Only the run of
/// identical digits STARTING at column 0 counts, and only if that run is longer
/// than 2. The line pays: bet * (sum of the digits in that run).
///
/// Line families:
///   1. Straight - one per row, straight across.
///   2. Zigzag   - one per starting row, bouncing down to the bottom row, up to
///                 the top row, and repeating.
///
/// Pure (no DB, no randomness) => trivially unit testable.
/// </summary>
public class WinCalculatorService : IWinCalculatorService
{
    public long CalculateWin(int[][] matrix, long bet)
    {
        if (matrix.Length == 0) return 0;

        int height = matrix.Length;
        int width = matrix[0].Length;
        long total = 0;

        // 1. Straight lines.
        for (int row = 0; row < height; row++)
            total += LineWin(BuildStraight(matrix, row, width), bet);

        // 2. Zigzag lines. Skipped when height == 1 (a zigzag would duplicate the
        //    straight line and double-count).
        if (height > 1)
            for (int start = 0; start < height; start++)
                total += LineWin(BuildZigzag(matrix, start, width, height), bet);

        return total;
    }

    private static long LineWin(int[] line, long bet)
    {
        int first = line[0];
        int count = 1;
        for (int i = 1; i < line.Length; i++)
        {
            if (line[i] == first) count++;
            else break;
        }

        // sum of identical digits = first * count
        return count >= GameConstants.MinWinningSeriesLength
            ? (long)first * count * bet
            : 0;
    }

    private static int[] BuildStraight(int[][] matrix, int row, int width)
    {
        var line = new int[width];
        for (int col = 0; col < width; col++)
            line[col] = matrix[row][col];
        return line;
    }

    /// <summary>
    /// Row index follows a triangle wave: down (+1) to the bottom, up (-1) to the
    /// top, repeating. Direction flips at a boundary before stepping.
    /// Example (height 3, width 5, start 0): rows 0,1,2,1,0.
    /// </summary>
    private static int[] BuildZigzag(int[][] matrix, int start, int width, int height)
    {
        var line = new int[width];
        int row = start;
        int direction = 1;

        for (int col = 0; col < width; col++)
        {
            line[col] = matrix[row][col];

            if (direction == 1 && row == height - 1) direction = -1;
            else if (direction == -1 && row == 0) direction = 1;

            row += direction;
        }
        return line;
    }
}