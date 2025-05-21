using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

using Library.API.IntegrationTests.Fixtures;

namespace Library.API.IntegrationTests.Api;

public abstract class IntegrationTestBase : IClassFixture<LibraryApiFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly LibraryApiFactory Factory;
    protected readonly Mock<IPublishEndpoint> PublishEndpointMock;
    private CancellationTokenSource? _cts;
    private const int TestTimeoutSeconds = 10;

    protected IntegrationTestBase(LibraryApiFactory factory)
    {
        Factory = factory;
        PublishEndpointMock = factory.GetPublishEndpointMock();
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

    public ValueTask InitializeAsync()
    {
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(TestTimeoutSeconds));
        PublishEndpointMock.Reset();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        return ValueTask.CompletedTask;
    }

    protected void VerifyEventPublished<T>(Times? times = null) where T : class
    {
        PublishEndpointMock.Verify(
            x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()),
            times ?? Times.Once());
    }

    protected void VerifyEventNotPublished<T>() where T : class
    {
        PublishEndpointMock.Verify(
            x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()),
            Times.Never());
    }
}