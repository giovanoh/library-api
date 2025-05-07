using Library.API.Domain.Models;
using Library.API.Infrastructure.Contexts;

using Microsoft.Extensions.DependencyInjection;

namespace Library.API.IntegrationTests.Fixtures;

public static class TestDataHelper
{
    public static void SeedAuthors(LibraryApiFactory factory, bool resetDatabase = false)
    {
        Author[] authors =
        [
            new Author
            {
                Id = 1,
                Name = "Author 1",
                Biography = "Biography 1",
                BirthDate = new DateTime(1980, 1, 1),
            },
            new Author
            {
                Id = 2,
                Name = "Author 2",
                Biography = "Biography 2",
                BirthDate = new DateTime(1990, 1, 1),
            }
        ];

        WithDbContext(factory, dbContext =>
        {
            if (resetDatabase)
            {
                dbContext.Database.EnsureDeleted();
            }
            dbContext.Database.EnsureCreated();

            dbContext.Authors.AddRange(authors);
            dbContext.SaveChanges();
        });
    }

    public static void SeedBooks(LibraryApiFactory factory, bool resetDatabase = false)
    {
        Book[] books =
        [
            new Book
            {
                Id = 1,
                Title = "Book 1",
                Description = "Description 1",
                AuthorId = 1,
                ReleaseDate = new DateTime(2000, 1, 1),
            },
            new Book
            {
                Id = 2,
                Title = "Book 2",
                Description = "Description 2",
                AuthorId = 2,
                ReleaseDate = new DateTime(2010, 1, 1),
            }
        ];

        WithDbContext(factory, dbContext =>
        {
            if (resetDatabase)
            {
                dbContext.Database.EnsureDeleted();
            }
            dbContext.Database.EnsureCreated();

            dbContext.Books.AddRange(books);
            dbContext.SaveChanges();
        });
    }

    private static void WithDbContext(LibraryApiFactory factory, Action<ApiDbContext> action)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        ApiDbContext dbContext = scopedServices.GetRequiredService<ApiDbContext>();

        action(dbContext);
    }
}