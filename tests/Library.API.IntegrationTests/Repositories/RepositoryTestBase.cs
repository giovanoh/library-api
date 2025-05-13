using Microsoft.EntityFrameworkCore;

using Library.API.Infrastructure.Contexts;

namespace Library.API.IntegrationTests.Repositories;

public abstract class RepositoryTestBase : IDisposable
{
    private readonly List<ApiDbContext> _contexts = [];

    protected ApiDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new ApiDbContext(options);
        _contexts.Add(context);
        return context;
    }

    public void Dispose()
    {
        foreach (var context in _contexts)
        {
            context.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}