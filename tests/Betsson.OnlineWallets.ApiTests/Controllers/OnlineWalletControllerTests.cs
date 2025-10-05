using System.Net;
using System.Net.Http.Json;
using Betsson.OnlineWallets.Models;
using Xunit;
using Xunit.Abstractions;

namespace Betsson.OnlineWallets.UnitTests;

public class OnlineWalletControllerTests
{
    private readonly string HTTP = "http://";
    private readonly string HOST = "localhost";
    private readonly string PORT = ":5000";
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;
    public OnlineWalletControllerTests(ITestOutputHelper output)
    {
        string url = string.Concat(HTTP, HOST, PORT);

        _httpClient = new HttpClient { BaseAddress = new Uri(url) };

        _output = output;
    }

    [Fact]
    public async Task GetBalance_ShouldReturn200()
    {
        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

        //Act
        _output.WriteLine("Executing GET request.");
        HttpResponseMessage response = await _httpClient.GetAsync("/onlinewallet/balance");

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
        decimal amount = 10;
        Balance balanceToDeposit = new Balance { Amount = amount };

        //Act
        _output.WriteLine("Executing POST request.");
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/onlinewallet/deposit", balanceToDeposit);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);

        //Arrange
        decimal expectedBalanceAmount = amount;

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
        _output.WriteLine("Undoing Deposit operation.");
        Withdrawal balanceToWithdraw = new Withdrawal { Amount = amount };

        await _httpClient.PostAsJsonAsync("/onlinewallet/withdraw", balanceToWithdraw);
    }

    [Fact]
    public async Task PostDeposit_WithInvalidValue_ShouldReturn400()
    {
        //Arrange
        HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        decimal amount = -10;
        Balance balanceToDeposit = new Balance { Amount = amount };

        //Act
        _output.WriteLine("Executing POST request.");
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/onlinewallet/deposit", balanceToDeposit);

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        _output.WriteLine("Expected Status: " + expectedStatusCode);
        _output.WriteLine("Current Status: " + currentStatusCode);
        Assert.Equal(expectedStatusCode, currentStatusCode);
    }
}