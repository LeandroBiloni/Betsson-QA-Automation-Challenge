public class SetupFixture
{
    private readonly string HTTP = "http://";
    private readonly string HOST = "localhost";
    private readonly string PORT = ":5000";
    private readonly string ROUTE = "/onlinewallet";
    public readonly string BALANCE_ENDPOINT;
    public readonly string DEPOSIT_ENDPOINT;
    public readonly string WITHDRAW_ENDPOINT;

    public readonly HttpClient httpClient;
    public SetupFixture()
    {
        string url = string.Concat(HTTP, HOST, PORT);

        httpClient = new HttpClient { BaseAddress = new Uri(url) };

        BALANCE_ENDPOINT = string.Concat(ROUTE, "/balance");
        DEPOSIT_ENDPOINT = string.Concat(ROUTE, "/deposit");
        WITHDRAW_ENDPOINT = string.Concat(ROUTE, "/withdraw");
    }
}