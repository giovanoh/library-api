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
} 