using Library.API.Domain.Models;

namespace Library.API.Tests.Helpers;

public static class TestDataHelper
{
    public static List<Author> Authors =>
    [
        new Author
        {
            Id = 1,
            Name = "Author 1",
            BirthDate = new DateTime(1980, 1, 1),
            Biography = "Author 1 is a software engineer."
        },
        new Author
        {
            Id = 2,
            Name = "Author 2",
            BirthDate = new DateTime(1985, 1, 1),
            Biography = "Author 2 is a software engineer."
        }
    ];

    public static List<Book> Books =>
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

    public static List<BookOrder> BookOrders =>
    [
        new BookOrder
        {
            Id = 1,
            CheckoutDate = new DateTime(2025, 5, 1),
            Status = BookOrderStatus.Placed,
            Items = [
                new BookOrderItem
                {
                    Id = 1,
                    BookId = 1,
                    Book = Books[0],
                    Quantity = 1,
                    BookOrderId = 1
                }
            ]
        }
    ];
}
