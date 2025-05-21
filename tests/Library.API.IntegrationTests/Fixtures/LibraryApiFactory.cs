using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MassTransit;
using Moq;

using Library.API.Infrastructure.Contexts;

namespace Library.API.IntegrationTests.Fixtures;

public class LibraryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"library_api_test_db_{Guid.NewGuid()}";
    private Mock<IPublishEndpoint> _publishEndpointMock = new();

    public Mock<IPublishEndpoint> GetPublishEndpointMock() => _publishEndpointMock;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ApiDbContext));
            if (dbDescriptor != null)
            {
                services.Remove(dbDescriptor);
                services.AddDbContext<ApiDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
            }

            var massTransitDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPublishEndpoint));
            if (massTransitDescriptor != null)
            {
                services.Remove(massTransitDescriptor);
                services.AddSingleton(_publishEndpointMock.Object);
            }

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            try
            {
                db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred seeding the database.", ex);
            }
        });
    }
}