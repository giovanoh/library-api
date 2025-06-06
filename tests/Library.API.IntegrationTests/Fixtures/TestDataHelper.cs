using Microsoft.Extensions.DependencyInjection;

using Library.API.Domain.Models;
using Library.API.Infrastructure.Contexts;

namespace Library.API.IntegrationTests.Fixtures;

public static class TestDataHelper
{
    private static Author[] GetDefaultAuthors() =>
    [
        new Author
        {
            Id = 1,
            Name = "Author 1",
            Biography = "Author 1 is a software engineer.",
            BirthDate = new DateTime(1980, 1, 1),
        },
        new Author
        {
            Id = 2,
            Name = "Author 2",
            Biography = "Author 2 is a software engineer.",
            BirthDate = new DateTime(1990, 1, 1),
        }
    ];

    public static void SeedAuthors(ApiDbContext dbContext, bool resetDatabase = false)
    {
        var authors = GetDefaultAuthors();
        SeedAuthorsInternal(dbContext, authors, resetDatabase);
    }

    public static void SeedAuthors(LibraryApiFactory factory, bool resetDatabase = false)
    {
        var authors = GetDefaultAuthors();

        WithDbContext(factory, dbContext =>
        {
            SeedAuthorsInternal(dbContext, authors, resetDatabase);
        });
    }

    private static void SeedAuthorsInternal(ApiDbContext dbContext, Author[] authors, bool resetDatabase = false)
    {
        if (resetDatabase)
        {
            dbContext.Database.EnsureDeleted();
        }
        dbContext.Database.EnsureCreated();

        dbContext.Authors.AddRange(authors);
        dbContext.SaveChanges();
    }

    private static Book[] GetDefaultBooks() =>
    [
        new Book
        {
            Id = 1,
            Title = "Book 1",
            Description = "Book 1 is a book about software engineering.",
            AuthorId = 1,
            ReleaseDate = new DateTime(2000, 1, 1),
        },
        new Book
        {
            Id = 2,
            Title = "Book 2",
            Description = "Book 2 is a book about software engineering.",
            AuthorId = 2,
            ReleaseDate = new DateTime(2010, 1, 1),
        }
    ];

    public static void SeedBooks(ApiDbContext dbContext, bool resetDatabase = false)
    {
        var books = GetDefaultBooks();
        SeedBooksInternal(dbContext, books, resetDatabase);
    }

    public static void SeedBooks(LibraryApiFactory factory, bool resetDatabase = false)
    {
        var books = GetDefaultBooks();

        WithDbContext(factory, dbContext =>
        {
            SeedBooksInternal(dbContext, books, resetDatabase);
        });
    }

    private static void SeedBooksInternal(ApiDbContext dbContext, Book[] books, bool resetDatabase = false)
    {
        if (resetDatabase)
        {
            dbContext.Database.EnsureDeleted();
        }
        dbContext.Database.EnsureCreated();

        dbContext.Books.AddRange(books);
        dbContext.SaveChanges();
    }

    private static BookOrder[] GetDefaultBookOrders() =>
    [
        new BookOrder
        {
            Id = 1,
            CheckoutDate = new DateTime(2020, 1, 1),
            Status = BookOrderStatus.Placed,
            Items = [
                new BookOrderItem
                {
                    Id = 1,
                    BookId = 1,
                    Quantity = 1,
                }
            ]
        }
    ];

    public static void SeedBookOrders(LibraryApiFactory factory, bool resetDatabase = false)
    {
        var bookOrders = GetDefaultBookOrders();
        WithDbContext(factory, dbContext =>
        {
            SeedBookOrdersInternal(dbContext, bookOrders, resetDatabase);
        });
    }

    public static void SeedBookOrders(ApiDbContext dbContext, bool resetDatabase = false)
    {
        var bookOrders = GetDefaultBookOrders();
        SeedBookOrdersInternal(dbContext, bookOrders, resetDatabase);
    }

    private static void SeedBookOrdersInternal(ApiDbContext dbContext, BookOrder[] bookOrders, bool resetDatabase = false)
    {
        if (resetDatabase)
        {
            dbContext.Database.EnsureDeleted();
        }
        dbContext.Database.EnsureCreated();

        dbContext.BookOrders.AddRange(bookOrders);
        dbContext.SaveChanges();
    }

    private static void WithDbContext(LibraryApiFactory factory, Action<ApiDbContext> action)
    {
        using IServiceScope scope = factory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;
        ApiDbContext dbContext = scopedServices.GetRequiredService<ApiDbContext>();

        action(dbContext);
    }
}