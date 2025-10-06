using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Services;
using Moq;

public class MockSetupFixture
{
    public readonly Mock<IOnlineWalletRepository> mockWalletRepository;
    public readonly IOnlineWalletService service;

    public MockSetupFixture()
    {
        mockWalletRepository = new Mock<IOnlineWalletRepository>();
        service = new OnlineWalletService(mockWalletRepository.Object);
    }
}