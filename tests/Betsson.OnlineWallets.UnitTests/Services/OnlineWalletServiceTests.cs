using Xunit;
using Moq;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Models;
using Xunit.Abstractions;
using Betsson.OnlineWallets.Models;

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

    [Fact]
    public async Task GetBalance_AfterFundsDeposit_ReturnsFunds()
    {
        //Arrange
        Deposit deposit = new Deposit { Amount = 10 };

        Mock<IOnlineWalletRepository> mockWalletRepository = new Mock<IOnlineWalletRepository>();

        //This will be the "last entry" created when depositing funds.
        OnlineWalletEntry lastWalletEntry = new OnlineWalletEntry();

        decimal expectedBalanceAmount = deposit.Amount + lastWalletEntry.Amount;

        mockWalletRepository
            .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
            .ReturnsAsync(lastWalletEntry);

        //Stores the deposited funds as lastWalletEntry.
        mockWalletRepository
            .Setup(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
            .Callback<OnlineWalletEntry>(entry =>
            {
                lastWalletEntry.Amount = entry.Amount;
                lastWalletEntry.BalanceBefore = entry.BalanceBefore;
            })
            .Returns(Task.CompletedTask);


        IOnlineWalletRepository repository = mockWalletRepository.Object;

        OnlineWalletService service = new OnlineWalletService(repository);
        
        Balance updatedBalance = await service.DepositFundsAsync(deposit);

        _output.WriteLine("Updated balance: " + updatedBalance.Amount);


        //Act
        Balance balanceAfterDeposit = await service.GetBalanceAsync();

        _output.WriteLine("Balance after deposit: " + balanceAfterDeposit.Amount);


        //Assert
        Assert.Equal(expectedBalanceAmount, balanceAfterDeposit.Amount);
    }
}