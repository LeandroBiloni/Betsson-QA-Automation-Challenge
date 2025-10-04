using Xunit;
using Moq;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Models;
using Xunit.Abstractions;

namespace Betsson.OnlineWallets.UnitTests;

public class OnlineWalletsServiceTests
{
    private readonly ITestOutputHelper _output;
    public OnlineWalletsServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task GetBalance_WithoutTransaction_ReturnsZero()
    {
        //Arrange
        decimal expectedBalanceAmount = 0;

        Mock<IOnlineWalletRepository> mockWalletRepository = new Mock<IOnlineWalletRepository>();

        mockWalletRepository
            .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
            .ReturnsAsync(new OnlineWalletEntry());

        IOnlineWalletRepository repository = mockWalletRepository.Object;

        OnlineWalletService service = new OnlineWalletService(repository);


        //Act
        var balance = await service.GetBalanceAsync();

        _output.WriteLine("Balance: " + balance.Amount);


        //Assert
        Assert.Equal(expectedBalanceAmount, balance.Amount);
    }
}