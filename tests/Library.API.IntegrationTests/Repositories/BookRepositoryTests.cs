using FluentAssertions;

using Library.API.Domain.Models;
using Library.API.Infrastructure.Repositories;
using Library.API.IntegrationTests.Fixtures;

using Microsoft.EntityFrameworkCore;

namespace Library.API.IntegrationTests.Repositories;

public class BookRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task AddAndListAsync_ShouldPersistAndReturnBooks()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var unitOfWork = new UnitOfWork(context);
        var bookRepository = new BookRepository(context);
        var authorRepository = new AuthorRepository(context);
        var author = new Author
        {
            Name = "Author 1",
            BirthDate = new DateTime(1980, 1, 1),
            Biography = "Author 1 is a software engineer."
        };

        // Act
        await authorRepository.AddAsync(author);
        var book = new Book
        {
            Title = "Book 1",
            AuthorId = author.Id,
            Description = "Book 1 is a book about software engineering.",
            ReleaseDate = new DateTime(2020, 1, 1)
        };
        await bookRepository.AddAsync(book);
        await unitOfWork.CompleteAsync();
        var bookResult = await bookRepository.ListAsync();

        // Assert book was persisted
        bookResult.Should().HaveCount(1);
        var retrievedBook = bookResult.First();
        retrievedBook.Id.Should().NotBe(0);
        retrievedBook.Title.Should().Be(book.Title);
        retrievedBook.AuthorId.Should().Be(author.Id);
        retrievedBook.Description.Should().Be(book.Description);
        retrievedBook.ReleaseDate.Should().Be(book.ReleaseDate);
        // Assert relationships
        retrievedBook.Author.Should().NotBeNull();
        retrievedBook.Author.Id.Should().Be(author.Id);
        retrievedBook.Author.Name.Should().Be(author.Name);
        retrievedBook.Author.BirthDate.Should().Be(author.BirthDate);
        retrievedBook.Author.Biography.Should().Be(author.Biography);
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsBook_WhenBookExists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        TestDataHelper.SeedAuthors(context);
        TestDataHelper.SeedBooks(context);
        var bookRepository = new BookRepository(context);        

        // Act
        var result = await bookRepository.FindByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Book 1");
        result.AuthorId.Should().Be(1);
        result.Description.Should().Be("Book 1 is a book about software engineering.");
        result.ReleaseDate.Should().Be(new DateTime(2000, 1, 1));
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var unitOfWork = new UnitOfWork(context);
        var bookRepository = new BookRepository(context);

        // Act
        var result = await bookRepository.FindByIdAsync(999); 

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBook_WhenBookExists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        TestDataHelper.SeedAuthors(context);
        TestDataHelper.SeedBooks(context);
        var unitOfWork = new UnitOfWork(context);
        var bookRepository = new BookRepository(context);

        // Act
        var book = await bookRepository.FindByIdAsync(1);
        book!.Title = "Updated Book 1";
        book.Description = "Updated description";
        book.ReleaseDate = new DateTime(2021, 1, 1);
        bookRepository.Update(book);
        await unitOfWork.CompleteAsync();

        // Assert
        var updatedBook = await bookRepository.FindByIdAsync(book.Id);
        updatedBook.Should().NotBeNull();
        updatedBook.Title.Should().Be(book.Title);
        updatedBook.AuthorId.Should().Be(book.AuthorId);
        updatedBook.Description.Should().Be(book.Description);
        updatedBook.ReleaseDate.Should().Be(book.ReleaseDate);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var unitOfWork = new UnitOfWork(context);
        var bookRepository = new BookRepository(context);
        var book = new Book
        {
            Id = 999,
            Title = "Book 1",
            AuthorId = 999,
            Description = "Book 999 is a book about software engineering.",
            ReleaseDate = new DateTime(2020, 1, 1)
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () =>
            {
                bookRepository.Update(book);
                await unitOfWork.CompleteAsync();
            })
            .Should()
            .ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task DeleteAsync_DeletesBook_WhenBookExists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        TestDataHelper.SeedAuthors(context);
        TestDataHelper.SeedBooks(context);
        var unitOfWork = new UnitOfWork(context);
        var bookRepository = new BookRepository(context);

        // Act
        var book = await bookRepository.FindByIdAsync(1);
        bookRepository.Delete(book!);
        await unitOfWork.CompleteAsync();

        // Assert
        var deletedBook = await bookRepository.FindByIdAsync(book!.Id);
        deletedBook.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenBookDoesNotExist()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var unitOfWork = new UnitOfWork(context);
        var bookRepository = new BookRepository(context);
        var book = new Book
        {
            Id = 999,
            Title = "Book 999",
            AuthorId = 999,
            Description = "Book 999 is a book about software engineering.",
            ReleaseDate = new DateTime(2020, 1, 1)
        };

        // Act & Assert 
        await FluentActions
            .Invoking(async () =>
            {
                bookRepository.Delete(book);
                await unitOfWork.CompleteAsync();
            })
            .Should()
            .ThrowAsync<DbUpdateException>();
    }
}
