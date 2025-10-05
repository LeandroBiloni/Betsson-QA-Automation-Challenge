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
        HttpResponseMessage response = await _httpClient.GetAsync("/onlinewallet/balance");

        HttpStatusCode currentStatusCode = response.StatusCode;

        //Assert
        Assert.Equal(expectedStatusCode, currentStatusCode);

        //Arrange
        decimal expectedBalanceAmount = 0;

        //Act
        Balance? balance = await response.Content.ReadFromJsonAsync<Balance>();

        //Assert
        Assert.NotNull(balance);
        
        decimal currentBalance = balance.Amount;
        _output.WriteLine("Current Balance: " + currentBalance);
        
        Assert.Equal(expectedBalanceAmount, currentBalance);
    }
}