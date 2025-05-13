using System.Diagnostics;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Library.API.Domain.Models;
using Library.API.Infrastructure.Repositories;
using Library.API.IntegrationTests.Fixtures;

namespace Library.API.IntegrationTests.Repositories;

public class AuthorRepositoryTests : RepositoryTestBase
{
    [Fact]
    public async Task AddAndListAsync_ShouldPersistAndReturnAuthors()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var authorRepository = new AuthorRepository(context, new ActivitySource("Library.API.IntegrationTests"));
        var unitOfWork = new UnitOfWork(context);
        var author = new Author
        {
            Name = "Author 1",
            BirthDate = new DateTime(1980, 1, 1),
            Biography = "Author 1 is a software engineer."
        };

        // Act
        await authorRepository.AddAsync(author);
        await unitOfWork.CompleteAsync();
        var result = await authorRepository.ListAsync();

        // Assert
        result.Should().HaveCount(1);
        var retrievedAuthor = result.First();
        retrievedAuthor.Name.Should().Be(author.Name);
        retrievedAuthor.BirthDate.Should().Be(author.BirthDate);
        retrievedAuthor.Biography.Should().Be(author.Biography);
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsAuthor_WhenAuthorExists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        TestDataHelper.SeedAuthors(context);
        var authorRepository = new AuthorRepository(context, new ActivitySource("Library.API.IntegrationTests"));
        var unitOfWork = new UnitOfWork(context);

        // Act
        var retrievedAuthor = await authorRepository.FindByIdAsync(1);

        // Assert
        retrievedAuthor.Should().NotBeNull();
        retrievedAuthor.Id.Should().Be(1);
        retrievedAuthor.Name.Should().Be("Author 1");
        retrievedAuthor.BirthDate.Should().Be(new DateTime(1980, 1, 1));
        retrievedAuthor.Biography.Should().Be("Author 1 is a software engineer.");
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsNull_WhenAuthorDoesNotExist()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var authorRepository = new AuthorRepository(context, new ActivitySource("Library.API.IntegrationTests"));

        // Act
        var found = await authorRepository.FindByIdAsync(999);

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAuthor_WhenAuthorExists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        TestDataHelper.SeedAuthors(context);
        var authorRepository = new AuthorRepository(context, new ActivitySource("Library.API.IntegrationTests"));
        var unitOfWork = new UnitOfWork(context);

        // Act
        var author = await authorRepository.FindByIdAsync(1);
        author.Should().NotBeNull();
        author.Name = "Updated Author";
        author.BirthDate = new DateTime(1985, 1, 1);
        author.Biography = "Updated biography.";
        authorRepository.Update(author);
        await unitOfWork.CompleteAsync();

        // Assert
        var updatedAuthor = await authorRepository.FindByIdAsync(author.Id);
        updatedAuthor.Should().NotBeNull();
        updatedAuthor.Name.Should().Be(author.Name);
        updatedAuthor.BirthDate.Should().Be(author.BirthDate);
        updatedAuthor.Biography.Should().Be(author.Biography);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenAuthorDoesNotExist()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var authorRepository = new AuthorRepository(context, new ActivitySource("Library.API.IntegrationTests"));
        var unitOfWork = new UnitOfWork(context);
        var author = new Author
        {
            Id = 999,
            Name = "Author 999",
            BirthDate = new DateTime(1980, 1, 1),
            Biography = "Author 999 is a software engineer."
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () =>
            {
                authorRepository.Update(author);
                await unitOfWork.CompleteAsync();
            })
            .Should()
            .ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task DeleteAsync_DeletesAuthor_WhenAuthorExists()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        TestDataHelper.SeedAuthors(context);
        var authorRepository = new AuthorRepository(context, new ActivitySource("Library.API.IntegrationTests"));
        var unitOfWork = new UnitOfWork(context);
        // Act
        var author = await authorRepository.FindByIdAsync(1);
        authorRepository.Delete(author!);
        await unitOfWork.CompleteAsync();

        // Assert
        var deletedAuthor = await authorRepository.FindByIdAsync(1);
        deletedAuthor.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenAuthorDoesNotExist()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var authorRepository = new AuthorRepository(context, new ActivitySource("Library.API.IntegrationTests"));
        var unitOfWork = new UnitOfWork(context);
        var author = new Author
        {
            Id = 999,
            Name = "Author 999",
            BirthDate = new DateTime(1980, 1, 1),
            Biography = "Author 999 is a software engineer."
        };

        // Act & Assert
        await FluentActions
            .Invoking(async () =>
            {
                authorRepository.Delete(author);
                await unitOfWork.CompleteAsync();
            })
            .Should()
            .ThrowAsync<DbUpdateException>();
    }

}