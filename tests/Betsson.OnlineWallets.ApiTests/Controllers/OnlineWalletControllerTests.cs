using System.Net;
using System.Net.Http.Json;
using Betsson.OnlineWallets.Models;
using Xunit;
using Xunit.Abstractions;

namespace Betsson.OnlineWallets.UnitTests;

public class OnlineWalletControllerTests : IClassFixture<SetupFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly SetupFixture _fixture;
    public OnlineWalletControllerTests(SetupFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task GetBalance_ShouldReturn200()
    {
        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

        //Act
        _output.WriteLine("Executing GET request.");
        HttpResponseMessage response = await _fixture.httpClient.GetAsync(_fixture.BALANCE_ENDPOINT);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);

        //Arrange
        decimal expectedBalanceAmount = 0;

        //Act
        _output.WriteLine("Reading response body.");
        Balance? balance = await response.Content.ReadFromJsonAsync<Balance>();

        //Assert
        Assert.NotNull(balance);

        decimal currentBalance = balance.Amount;

        _output.WriteLine("Expected Balance: " + expectedBalanceAmount);
        _output.WriteLine("Current Balance: " + currentBalance);
        Assert.Equal(expectedBalanceAmount, currentBalance);
    }

    [Fact]
    public async Task PostDeposit_WithValidAmount_ShouldReturn200_AndDepositedAmount()
    {
        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
        decimal amountToDeposit = 10;
        Balance balanceToDeposit = new Balance { Amount = amountToDeposit };

        //Act
        _output.WriteLine("Executing POST request.");
        HttpResponseMessage response = await _fixture.httpClient.PostAsJsonAsync(_fixture.DEPOSIT_ENDPOINT, balanceToDeposit);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);

        //Arrange
        decimal expectedBalanceAmount = amountToDeposit;

        //Act
        _output.WriteLine("Reading response body.");
        Balance? balance = await response.Content.ReadFromJsonAsync<Balance>();

        //Assert
        Assert.NotNull(balance);

        decimal currentBalance = balance.Amount;

        _output.WriteLine("Expected Balance: " + expectedBalanceAmount);
        _output.WriteLine("Current Balance: " + currentBalance);
        Assert.Equal(expectedBalanceAmount, currentBalance);

        //Reset
        _output.WriteLine("Test Reset - Undoing Deposit operation.");
        Withdrawal balanceToWithdraw = new Withdrawal { Amount = amountToDeposit };

        await _fixture.httpClient.PostAsJsonAsync(_fixture.WITHDRAW_ENDPOINT, balanceToWithdraw);
    }

    [Fact]
    public async Task PostDeposit_WithInvalidValue_ShouldReturn400()
    {
        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        decimal amountToDeposit = -10;
        Balance balanceToDeposit = new Balance { Amount = amountToDeposit };

        //Act
        _output.WriteLine("Executing POST request.");
        HttpResponseMessage response = await _fixture.httpClient.PostAsJsonAsync(_fixture.DEPOSIT_ENDPOINT, balanceToDeposit);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);
    }

    [Fact]
    public async Task PostWithdraw_WithInvalidValue_ShouldReturn400()
    {
        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        decimal amountToWithdraw = -10;
        Withdrawal balanceToWithdraw = new Withdrawal { Amount = amountToWithdraw };

        //Act
        _output.WriteLine("Executing POST request.");
        HttpResponseMessage response = await _fixture.httpClient.PostAsJsonAsync(_fixture.WITHDRAW_ENDPOINT, balanceToWithdraw);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);
    }

    [Fact]
    public async Task PostWithdraw_WithInsufficientFunds_ShouldReturn400()
    {
        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        decimal amountToWithdraw = 50;
        Withdrawal balanceToWithdraw = new Withdrawal { Amount = amountToWithdraw };

        //Act
        _output.WriteLine("Executing POST request.");
        HttpResponseMessage response = await _fixture.httpClient.PostAsJsonAsync(_fixture.WITHDRAW_ENDPOINT, balanceToWithdraw);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);
    }

    [Fact]
    public async Task PostWithdraw_WithEnoughFunds_ShouldReturn200_AndUpdatedAmount()
    {
        //Setup
        _output.WriteLine("Test Setup - Depositing funds.");
        decimal amountToDeposit = 20;
        Balance balanceToDeposit = new Balance { Amount = amountToDeposit };
        await _fixture.httpClient.PostAsJsonAsync(_fixture.DEPOSIT_ENDPOINT, balanceToDeposit);

        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
        decimal amountToWithdraw = 10;
        Withdrawal balanceToWithdraw = new Withdrawal { Amount = amountToWithdraw };

        //Act
        _output.WriteLine("Executing POST request.");
        HttpResponseMessage response = await _fixture.httpClient.PostAsJsonAsync(_fixture.WITHDRAW_ENDPOINT, balanceToWithdraw);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);

        //Arrange
        decimal expectedBalanceAmount = amountToDeposit - amountToWithdraw;

        //Act
        _output.WriteLine("Reading response body.");
        Balance? balance = await response.Content.ReadFromJsonAsync<Balance>();

        //Assert
        Assert.NotNull(balance);

        decimal currentBalance = balance.Amount;

        _output.WriteLine("Expected Balance: " + expectedBalanceAmount);
        _output.WriteLine("Current Balance: " + currentBalance);
        Assert.Equal(expectedBalanceAmount, currentBalance);

        //Reset
        _output.WriteLine("Test Reset - Withdrawing remaining funds.");
        Withdrawal balanceReset = new Withdrawal { Amount = currentBalance };

        await _fixture.httpClient.PostAsJsonAsync(_fixture.WITHDRAW_ENDPOINT, balanceReset);
    }
}