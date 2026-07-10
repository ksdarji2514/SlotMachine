using SlotMachine.Service.Interfaces;
using SlotMachine.Service.Services;

namespace SlotMachine.Tests;

public class WinCalculatorServiceTests
{
    private readonly IWinCalculatorService _calc = new WinCalculatorService();
    // A 3 row, 5 column matrix from the PDF example, with a bet of 1. 
    //   3 3 3 4 5
    //   2 3 2 3 3
    //   1 2 3 3 3
    // Paying lines: straight row 0 (9) + zigzag start 0 (12) + zigzag start 1 (6) = 27.
    [Fact]
    public void Example_TotalIs270_WhenBetIs10()
    {
        int[][] matrix =
        {
            new[] { 3, 3, 3, 4, 5 },
            new[] { 2, 3, 2, 3, 3 },
            new[] { 1, 2, 3, 3, 3 },
        };

        long win = _calc.CalculateWin(matrix, bet: 10);

        Assert.Equal(270, win);
    }


    // The single-line rules given in the PDF, tested via a 1-row matrix
    // (one straight line, no zigzags).
    [Theory]
    [InlineData(new[] { 3, 3, 3, 4, 5 }, 9)]         // three leading 3s
    [InlineData(new[] { 2, 3, 2 }, 0)]               // run length 1
    [InlineData(new[] { 7, 7, 7, 3, 7, 7, 3 }, 21)]  // only the leading 7s count
    public void SingleRow_AppliesConsecutiveRule(int[] row, long expected)
    {
        int[][] matrix = { row };
        Assert.Equal(expected, _calc.CalculateWin(matrix, bet: 1));
    }

    [Fact]
    public void RunOfExactlyTwo_DoesNotPay()
    {
        int[][] matrix = { new[] { 5, 5, 1, 1, 1 } }; // leading run is length 2
        Assert.Equal(0, _calc.CalculateWin(matrix, bet: 1));
    }

    [Fact]
    public void RunMustStartAtColumnZero()
    {
        int[][] matrix = { new[] { 1, 8, 8, 8, 8 } }; // the 8s don't start at col 0
        Assert.Equal(0, _calc.CalculateWin(matrix, bet: 1));
    }

    [Fact]
    public void AllZeros_PayZero_EvenWithLongRun()
    {
        int[][] matrix = { new[] { 0, 0, 0, 0, 0 } }; // sum = 0 * count = 0
        Assert.Equal(0, _calc.CalculateWin(matrix, bet: 5));
    }

    [Fact]
    public void FullRowSameDigit_PaysForWholeRow()
    {
        int[][] matrix = { new[] { 4, 4, 4, 4, 4 } }; // 4*5 = 20, * bet 2 = 40
        Assert.Equal(40, _calc.CalculateWin(matrix, bet: 2));
    }

    [Fact]
    public void EmptyMatrix_PaysZero()
    {
        Assert.Equal(0, _calc.CalculateWin(System.Array.Empty<int[]>(), bet: 10));
    }
    [Fact]
    public void ThreeFullRows_OnlyStraightLinesPay_Totals48()
    {
        // 3 3 3 3 5   -> straight pays 3*4 = 12
        // 4 4 4 4 5   -> straight pays 4*4 = 16
        // 5 5 5 5 3   -> straight pays 5*4 = 20
        // all zigzags break at column 1 -> 0
        int[][] matrix =
        {
        new[] { 3, 3, 3, 3, 5 },
        new[] { 4, 4, 4, 4, 5 },
        new[] { 5, 5, 5, 5, 3 },
    };

        Assert.Equal(48, _calc.CalculateWin(matrix, bet: 1));
        Assert.Equal(480, _calc.CalculateWin(matrix, bet: 10));
    }
}