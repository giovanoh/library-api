using Library.API.IntegrationTests.Fixtures;

using Microsoft.AspNetCore.Mvc.Testing;

namespace Library.API.IntegrationTests.Api;

public class IntegrationTestBase : IClassFixture<LibraryApiFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly LibraryApiFactory Factory;
    private CancellationTokenSource? _cts;
    private const int TestTimeoutSeconds = 10;

    protected IntegrationTestBase(LibraryApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

    public ValueTask InitializeAsync()
    {
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(TestTimeoutSeconds));
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        return ValueTask.CompletedTask;
    }
}