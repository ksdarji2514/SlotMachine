using SlotMachine.Common.Constants;
using SlotMachine.Common.Exceptions;
using SlotMachine.DTO.Requests;
using SlotMachine.DTO.Responses;
using SlotMachine.Repository.Interfaces;
using SlotMachine.Service.Interfaces;

namespace SlotMachine.Service.Services;

public class SlotService(IConfigurationRepository _config, IPlayerRepository _players, IWinCalculatorService _winCalculator) : ISlotService
{
   
    public async Task<SpinResponse> SpinAsync(SpinRequest request)
    {
        // 1. Atomic check-and-deduct.
        var afterDeduct = await _players.TryDeductBetAsync(request.PlayerId, request.Bet);
        if (afterDeduct is null)
        {
            // The atomic deduct failed. Look up the player to find out why.
            var player = await _players.GetAsync(request.PlayerId);

            if (player is null)
                throw new PlayerNotFoundException(
                    ErrorMessages.PlayerNotFoundMessage);

            throw new InsufficientBalanceException(
                ErrorMessages.InsufficientBalanceMessage);
        }
        // 2. Read config fresh => reconfigurable without restart.
        var config = await _config.GetAsync()
            ?? throw new InvalidOperationException(
                ErrorMessages.SlotConfigurationNotFoundMessage);

        // 3. Random result matrix.
        var matrix = GenerateMatrix(config.Width, config.Height);

        // 4. Pure win calculation.
        long win = _winCalculator.CalculateWin(matrix, request.Bet);

        // 5. Atomic credit.
        var afterWin = await _players.CreditWinAsync(request.PlayerId, win);

        // 6. Map entity -> DTO.
        return new SpinResponse
        {
            Matrix = matrix,
            Win = win,
            Balance = afterWin.Balance
        };
    }
    
    private static int[][] GenerateMatrix(int width, int height)
    {
        var matrix = new int[height][];
        for (int row = 0; row < height; row++)
        {
            matrix[row] = new int[width];
            for (int col = 0; col < width; col++)
                matrix[row][col] = Random.Shared.Next(
                    GameConstants.MinDigit, GameConstants.MaxDigitExclusive);
        }
        return matrix;
    }
}
