using Xunit;
using Xunit.Abstractions;

namespace Betsson.OnlineWallets.ApiTests;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;
    
    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test1()
    {
        _output.WriteLine("API tests");

        Assert.True(false);
    }
}