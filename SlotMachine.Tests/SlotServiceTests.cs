using Moq;
using SlotMachine.Common.Exceptions;
using SlotMachine.Database.Entities;
using SlotMachine.DTO.Requests;
using SlotMachine.Repository.Interfaces;
using SlotMachine.Service.Interfaces;
using SlotMachine.Service.Services;

namespace SlotMachine.Tests;

public class SlotServiceTests
{
    private readonly Mock<IPlayerRepository> _players = new();
    private readonly Mock<IConfigurationRepository> _config = new();
    private readonly IWinCalculatorService  _winCalculator = new WinCalculatorService();

    private SlotService CreateService() =>
        new SlotService( _config.Object, _players.Object, _winCalculator);

    [Fact]
    public async Task Spin_WhenPlayerNotFound_ThrowsPlayerNotFound_AndDoesNotCreditWin()
    {
        _players
            .Setup(r => r.TryDeductBetAsync(It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync((Player?)null);

        // GetAsync returns null => player genuinely doesn't exist.
        _players
            .Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((Player?)null);

        var service = CreateService();
        var request = new SpinRequest { PlayerId = "krina-1", Bet = 100 };

        await Assert.ThrowsAsync<PlayerNotFoundException>(
            () => service.SpinAsync(request));

        _players.Verify(r => r.CreditWinAsync(It.IsAny<string>(), It.IsAny<long>()),
                        Times.Never);
    }

    [Fact]
    public async Task Spin_WhenInsufficientBalance_ThrowsInsufficientBalance_AndDoesNotCreditWin()
    {
        _players
            .Setup(r => r.TryDeductBetAsync(It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync((Player?)null);

        // GetAsync returns a real player => they exist, so it's a balance problem.
        _players
            .Setup(r => r.GetAsync("krina-1"))
            .ReturnsAsync(new Player { Id = "krina-1", Balance = 50 });

        var service = CreateService();
        var request = new SpinRequest { PlayerId = "krina-1", Bet = 100 };

        await Assert.ThrowsAsync<InsufficientBalanceException>(
            () => service.SpinAsync(request));

        _players.Verify(r => r.CreditWinAsync(It.IsAny<string>(), It.IsAny<long>()),
                        Times.Never);
    }
    [Fact]
    public async Task Spin_WhenConfigMissing_Throws()
    {
        _players
            .Setup(r => r.TryDeductBetAsync("krina-1", 10))
            .ReturnsAsync(new Player { Id = "krina-1", Balance = 90 });

        _config
            .Setup(r => r.GetAsync())
            .ReturnsAsync((GameConfiguration?)null);

        var service = CreateService();
        var request = new SpinRequest { PlayerId = "krina-1", Bet = 10 };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SpinAsync(request));
    }

    [Fact]
    public async Task Spin_Successful_CreditsWinAndReturnsBalance()
    {
        // Arrange: player has funds, config is 5x3.
        _players
            .Setup(r => r.TryDeductBetAsync("krina-1", 10))
            .ReturnsAsync(new Player { Id = "krina-1", Balance = 90 });

        _config
            .Setup(r => r.GetAsync())
            .ReturnsAsync(new GameConfiguration { Id = "default", Width = 5, Height = 3 });

        // Whatever win is computed, CreditWin returns the resulting balance.
        _players
            .Setup(r => r.CreditWinAsync("krina-1", It.IsAny<long>()))
            .ReturnsAsync((string _, long win) =>
                new Player { Id = "krina-1", Balance = 90 + win });

        var service = CreateService();
        var request = new SpinRequest { PlayerId = "krina-1", Bet = 10 };

        // Act
        var result = await service.SpinAsync(request);

        // Assert: matrix has the configured shape, and balance = 90 + win.
        Assert.Equal(3, result.Matrix.Length);      // height
        Assert.Equal(5, result.Matrix[0].Length);   // width
        Assert.Equal(90 + result.Win, result.Balance);

        _players.Verify(r => r.CreditWinAsync("krina-1", result.Win), Times.Once);
    }
}