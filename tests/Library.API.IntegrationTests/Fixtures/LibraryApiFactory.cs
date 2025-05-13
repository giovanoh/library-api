using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Library.API.Infrastructure.Contexts;

namespace Library.API.IntegrationTests.Fixtures;

public class LibraryApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"library_api_test_db_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ApiDbContext));
            if (descriptor != null)
            {
                services.Remove(descriptor);
                services.AddDbContext<ApiDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
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