using Xunit;
using Moq;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Models;
using Xunit.Abstractions;
using Betsson.OnlineWallets.Models;
using Xunit.Sdk;
using Betsson.OnlineWallets.Exceptions;

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
        _output.WriteLine("Getting balance.");
        var balance = await service.GetBalanceAsync();

        //Assert
        _output.WriteLine("Expected Balance: " + expectedBalanceAmount);
        _output.WriteLine("Current Balance: " + balance.Amount);
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

        _output.WriteLine("Depositing balance.");
        Balance updatedBalance = await service.DepositFundsAsync(deposit);

        //Act
        _output.WriteLine("Getting balance.");
        Balance balanceAfterDeposit = await service.GetBalanceAsync();

        //Assert
        _output.WriteLine("Expected Balance: " + expectedBalanceAmount);
        _output.WriteLine("Current Balance: " + balanceAfterDeposit.Amount);
        Assert.Equal(expectedBalanceAmount, balanceAfterDeposit.Amount);
    }

    [Fact]
    public async Task DepositFunds_PositiveAmount_ReturnsUpdatedBalance()
    {
        //Arrange
        Deposit deposit = new Deposit { Amount = 10 };

        Mock<IOnlineWalletRepository> mockWalletRepository = new Mock<IOnlineWalletRepository>();

        decimal expectedBalanceAmount = deposit.Amount;

        mockWalletRepository
            .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
            .ReturnsAsync(new OnlineWalletEntry());

        mockWalletRepository
            .Setup(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
            .Returns(Task.CompletedTask);


        IOnlineWalletRepository repository = mockWalletRepository.Object;

        OnlineWalletService service = new OnlineWalletService(repository);

        //Act
        _output.WriteLine("Depositing funds.");
        Balance updatedBalance = await service.DepositFundsAsync(deposit);

        //Assert
        _output.WriteLine("Expected Balance: " + expectedBalanceAmount);
        _output.WriteLine("Current Balance: " + updatedBalance.Amount);
        Assert.Equal(expectedBalanceAmount, updatedBalance.Amount);
    }

    [Fact]
    public async Task DepositFunds_WithPreviousBalance_ReturnsUpdatedBalance()
    {
        //Arrange
        Deposit deposit = new Deposit { Amount = 10 };

        Mock<IOnlineWalletRepository> mockWalletRepository = new Mock<IOnlineWalletRepository>();

        OnlineWalletEntry previousEntryWithBalance = new OnlineWalletEntry { Amount = 10 };

        decimal expectedBalanceAmount = deposit.Amount + previousEntryWithBalance.Amount;

        mockWalletRepository
            .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
            .ReturnsAsync(previousEntryWithBalance);

        mockWalletRepository
            .Setup(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
            .Returns(Task.CompletedTask);


        IOnlineWalletRepository repository = mockWalletRepository.Object;

        OnlineWalletService service = new OnlineWalletService(repository);

        //Act
        _output.WriteLine("Depositing funds.");
        Balance updatedBalance = await service.DepositFundsAsync(deposit);

        //Assert
        _output.WriteLine("Expected Balance: " + expectedBalanceAmount);
        _output.WriteLine("Current Balance: " + updatedBalance.Amount);
        Assert.Equal(expectedBalanceAmount, updatedBalance.Amount);
    }

    //This test will fail, it's created considering the future implementation of a validation of a negative or zero amount to deposit.
    [Fact]
    public async Task DepositFunds_WithNegativeAmount_ShouldThrowException()
    {
        //Arrange
        Deposit deposit = new Deposit { Amount = -10 };

        Mock<IOnlineWalletRepository> mockWalletRepository = new Mock<IOnlineWalletRepository>();

        IOnlineWalletRepository repository = mockWalletRepository.Object;

        OnlineWalletService service = new OnlineWalletService(repository);

        //Act
        Func<Task> depositFunc = async () =>
        {
            _output.WriteLine("Depositing funds.");
            await service.DepositFundsAsync(deposit);
        };

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(depositFunc);
    }

    [Fact]
    public async Task WithdrawFunds_WithInsufficientBalance_ShouldThrowException()
    {
        //Arrange
        Withdrawal withdrawal = new Withdrawal { Amount = 10 };

        Mock<IOnlineWalletRepository> mockWalletRepository = new Mock<IOnlineWalletRepository>();

        mockWalletRepository
            .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
            .ReturnsAsync(new OnlineWalletEntry());

        IOnlineWalletRepository repository = mockWalletRepository.Object;

        OnlineWalletService service = new OnlineWalletService(repository);

        //Act
        Func<Task> withdraw = async () =>
        {
            _output.WriteLine("Withdrawing funds.");
            await service.WithdrawFundsAsync(withdrawal);
        };

        //Assert
        await Assert.ThrowsAsync<InsufficientBalanceException>(withdraw);
    }

    //This test will fail, it's created considering the future implementation of a validation of a negative or zero amount to withdraw.
    [Fact]
    public async Task WithdrawFunds_WithNegativeValue_ShouldThrowException()
    {
        //Arrange
        Withdrawal withdrawal = new Withdrawal { Amount = -10 };

        Mock<IOnlineWalletRepository> mockWalletRepository = new Mock<IOnlineWalletRepository>();

        mockWalletRepository
            .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
            .ReturnsAsync(new OnlineWalletEntry());

        IOnlineWalletRepository repository = mockWalletRepository.Object;

        OnlineWalletService service = new OnlineWalletService(repository);

        //Act
        Func<Task> withdraw = async () =>
        {
            _output.WriteLine("Withdrawing funds.");
            await service.WithdrawFundsAsync(withdrawal);
        };

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(withdraw);
    }    
}