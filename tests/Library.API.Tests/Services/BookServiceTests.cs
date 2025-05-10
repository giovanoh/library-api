using FluentAssertions;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Domain.Services.Communication;
using Library.API.Infrastructure.Services;
using Library.API.Tests.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

namespace Library.API.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<BookService>> _loggerMock;

    public BookServiceTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<BookService>>();
    }

    private BookService CreateService() =>
        new BookService(_bookRepositoryMock.Object, _authorRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);

    [Fact]
    public async Task ListAsync_ShouldReturnBooks_WhenSuccessful()
    {
        // Arrange
        BookService service = CreateService();
        _bookRepositoryMock.Setup(repo => repo.ListAsync()).ReturnsAsync(TestDataHelper.Books);

        // Act
        Response<IEnumerable<Book>> result = await service.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().HaveCount(2);
        result.Model.Should().BeEquivalentTo(TestDataHelper.Books);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookRepositoryMock.Verify(repo => repo.ListAsync(), Times.Once);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        BookService service = CreateService();
        _bookRepositoryMock.Setup(repo => repo.ListAsync()).ThrowsAsync(new Exception("Repository error"));

        // Act
        Response<IEnumerable<Book>> result = await service.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while retrieving the books");

        _bookRepositoryMock.Verify(repo => repo.ListAsync(), Times.Once);
        _loggerMock.Verify(
            static x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Error occurred while listing books")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnBook_WhenSuccessful()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);

        // Act
        Response<Book> result = await service.FindByIdAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(book);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnError_WhenBookNotFound()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync((Book?)null);

        // Act
        Response<Book> result = await service.FindByIdAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Book with id {book.Id} was not found");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ThrowsAsync(new Exception("Repository error"));

        // Act
        Response<Book> result = await service.FindByIdAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while retrieving the book");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Error occurred while finding book")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddAsync_ShouldReturnBook_WhenSuccessful()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        book.Author = TestDataHelper.Authors.FirstOrDefault(a => a.Id == book.AuthorId)!;
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync(book.Author);
        _bookRepositoryMock.Setup(repo => repo.AddAsync(book)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        Response<Book> result = await service.AddAsync(book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(book);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookRepositoryMock.Verify(repo => repo.AddAsync(book), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(book.AuthorId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        book.Author = TestDataHelper.Authors.FirstOrDefault(a => a.Id == book.AuthorId)!;
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync(book.Author);
        _bookRepositoryMock.Setup(repo => repo.AddAsync(book)).ThrowsAsync(new DbUpdateException("Repository error"));

        // Act
        Response<Book> result = await service.AddAsync(book);

        // Assert   
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while saving the book");

        _bookRepositoryMock.Verify(repo => repo.AddAsync(book), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(book.AuthorId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Error occurred while saving book")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenUnitOfWorkFails()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        book.Author = TestDataHelper.Authors.FirstOrDefault(a => a.Id == book.AuthorId)!;
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync(book.Author);
        _bookRepositoryMock.Setup(repo => repo.AddAsync(book)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).ThrowsAsync(new DbUpdateException("Unit of work error"));

        // Act
        Response<Book> result = await service.AddAsync(book);

        // Assert   
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while saving the book");

        _bookRepositoryMock.Verify(repo => repo.AddAsync(book), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(book.AuthorId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Error occurred while saving book")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        book.Author = TestDataHelper.Authors.FirstOrDefault(a => a.Id == book.AuthorId)!;
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync(book.Author);
        _bookRepositoryMock.Setup(repo => repo.AddAsync(book)).Throws(new Exception("Unexpected error"));

        // Act
        Response<Book> result = await service.AddAsync(book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.Unknown);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An unexpected error occurred while processing your request");

        _bookRepositoryMock.Verify(repo => repo.AddAsync(book), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(book.AuthorId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Unexpected error occurred while adding book.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenAuthorNotFound()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync((Author?)null);

        // Act
        Response<Book> result = await service.AddAsync(book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Author with id {book.AuthorId} was not found");

        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(book.AuthorId), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.AddAsync(book), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnBook_WhenSuccessful()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        book.Author = TestDataHelper.Authors.FirstOrDefault(a => a.Id == book.AuthorId)!;
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync(book.Author);
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(repo => repo.Update(book));
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        Response<Book> result = await service.UpdateAsync(book.Id, book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(book);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Update(book), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenBookNotFound()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync((Book?)null);

        // Act  
        Response<Book> result = await service.UpdateAsync(book.Id, book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Book with id {book.Id} was not found");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Update(book), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenAuthorNotFound()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync((Author?)null);

        // Act
        Response<Book> result = await service.UpdateAsync(book.Id, book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Author with id {book.AuthorId} was not found");

        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(book.AuthorId), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Update(book), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange  
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        book.Author = TestDataHelper.Authors.FirstOrDefault(a => a.Id == book.AuthorId)!;
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync(book.Author);
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(repo => repo.Update(book)).Throws(new DbUpdateException("Repository error"));

        // Act
        Response<Book> result = await service.UpdateAsync(book.Id, book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while updating the book");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Update(book), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(book.AuthorId), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Error occurred while updating book")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        book.Author = TestDataHelper.Authors.FirstOrDefault(a => a.Id == book.AuthorId)!;
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(book.AuthorId)).ReturnsAsync(book.Author);
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(repo => repo.Update(book)).Throws(new Exception("Unexpected error"));

        // Act
        Response<Book> result = await service.UpdateAsync(book.Id, book);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.Unknown);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An unexpected error occurred while processing your request");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Update(book), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Unexpected error occurred while updating book")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnBook_WhenSuccessful()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(repo => repo.Delete(book));
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        Response<Book> result = await service.DeleteAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(book);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Delete(book), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenBookNotFound()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync((Book?)null);

        // Act
        Response<Book> result = await service.DeleteAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Book with id {book.Id} was not found");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Delete(book), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(repo => repo.Delete(book)).Throws(new DbUpdateException("Repository error"));

        // Act
        Response<Book> result = await service.DeleteAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while deleting the book");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Delete(book), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Error occurred while deleting book")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        BookService service = CreateService();
        Book book = TestDataHelper.Books[0];
        _bookRepositoryMock.Setup(repo => repo.FindByIdAsync(book.Id)).ReturnsAsync(book);
        _bookRepositoryMock.Setup(repo => repo.Delete(book)).Throws(new Exception("Unexpected error"));

        // Act
        Response<Book> result = await service.DeleteAsync(book.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.Unknown);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An unexpected error occurred while processing your request");

        _bookRepositoryMock.Verify(repo => repo.FindByIdAsync(book.Id), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.Delete(book), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains("Unexpected error occurred while deleting book")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
}