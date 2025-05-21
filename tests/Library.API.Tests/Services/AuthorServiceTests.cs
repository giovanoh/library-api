using System.Diagnostics;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

using Library.API.Domain.Models;
using Library.API.Domain.Repositories;
using Library.API.Domain.Services.Communication;
using Library.API.Infrastructure.Services;
using Library.API.Tests.Helpers;

namespace Library.API.Tests.Services;

public class AuthorServiceTests : ServiceTestBase<AuthorService>
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ActivitySource _activitySource;
    public AuthorServiceTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _activitySource = new ActivitySource("Library.API.Tests");
    }

    private AuthorService CreateService() =>
        new AuthorService(_authorRepositoryMock.Object, _unitOfWorkMock.Object, LoggerMock.Object, _activitySource);

    [Fact]
    public async Task ListAsync_ShouldReturnAuthors_WhenSuccessful()
    {
        // Arrange
        AuthorService service = CreateService();
        _authorRepositoryMock.Setup(repo => repo.ListAsync()).ReturnsAsync(TestDataHelper.Authors);

        // Act
        Response<IEnumerable<Author>> result = await service.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().HaveCount(2);
        result.Model.Should().BeEquivalentTo(TestDataHelper.Authors);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _authorRepositoryMock.Verify(repo => repo.ListAsync(), Times.Once);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        AuthorService service = CreateService();
        _authorRepositoryMock.Setup(repo => repo.ListAsync()).ThrowsAsync(new Exception("Repository error"));

        // Act
        Response<IEnumerable<Author>> result = await service.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while retrieving the authors");

        _authorRepositoryMock.Verify(repo => repo.ListAsync(), Times.Once);
        VerifyErrorLog("Error occurred while listing authors");
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnAuthor_WhenSuccessful()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync(author);

        // Act
        Response<Author> result = await service.FindByIdAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(author);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnError_WhenAuthorNotFound()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync((Author?)null);

        // Act
        Response<Author> result = await service.FindByIdAsync(author.Id);

        // Assert   
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Author with id {author.Id} was not found");

        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ThrowsAsync(new Exception("Repository error"));

        // Act
        Response<Author> result = await service.FindByIdAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while retrieving the author");

        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        VerifyErrorLog("Error occurred while finding author");
    }

    [Fact]
    public async Task AddAsync_ShouldReturnAuthor_WhenSuccessful()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.AddAsync(author));
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        Response<Author> result = await service.AddAsync(author);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(author);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _authorRepositoryMock.Verify(repo => repo.AddAsync(author), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.AddAsync(author)).ThrowsAsync(new DbUpdateException("Repository error"));

        // Act
        Response<Author> result = await service.AddAsync(author);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while saving the author");

        _authorRepositoryMock.Verify(repo => repo.AddAsync(author), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        VerifyErrorLog("Error occurred while saving author");
    }

    [Fact]
    public async Task AddAsync_ShouldReturnError_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.AddAsync(author)).ThrowsAsync(new Exception("Unexpected error"));

        // Act
        Response<Author> result = await service.AddAsync(author);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.Unknown);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An unexpected error occurred while processing your request");

        _authorRepositoryMock.Verify(repo => repo.AddAsync(author), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        VerifyErrorLog("Unexpected error occurred while adding author.");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnAuthor_WhenSuccessful()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync(author);
        _authorRepositoryMock.Setup(repo => repo.Update(author));
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        Response<Author> result = await service.UpdateAsync(author.Id, author);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(author);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _authorRepositoryMock.Verify(repo => repo.Update(author), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenAuthorNotFound()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync((Author?)null);

        // Act
        Response<Author> result = await service.UpdateAsync(author.Id, author);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Author with id {author.Id} was not found");

        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.Update(It.IsAny<Author>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync(author);
        _authorRepositoryMock.Setup(repo => repo.Update(author)).Throws(new DbUpdateException("Repository error"));

        // Act
        Response<Author> result = await service.UpdateAsync(author.Id, author);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while updating the author");

        _authorRepositoryMock.Verify(repo => repo.Update(author), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        VerifyErrorLog("Error occurred while updating author");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync(author);
        _authorRepositoryMock.Setup(repo => repo.Update(author)).Throws(new Exception("Unexpected error"));

        // Act  
        Response<Author> result = await service.UpdateAsync(author.Id, author);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.Unknown);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An unexpected error occurred while processing your request");

        _authorRepositoryMock.Verify(repo => repo.Update(author), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        VerifyErrorLog($"Unexpected error occurred while updating author {author.Id}.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnAuthor_WhenSuccessful()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync(author);
        _authorRepositoryMock.Setup(repo => repo.Delete(author));
        _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

        // Act
        Response<Author> result = await service.DeleteAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Model.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(author);
        result.Error.Should().BeNull();
        result.Message.Should().BeNull();

        _authorRepositoryMock.Verify(repo => repo.Delete(author), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenAuthorNotFound()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync((Author?)null);

        // Act
        Response<Author> result = await service.DeleteAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.NotFound);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be($"Author with id {author.Id} was not found");

        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Author>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenRepositoryFails()
    {
        // Arrange
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync(author);
        _authorRepositoryMock.Setup(repo => repo.Delete(author)).Throws(new DbUpdateException("Repository error"));

        // Act
        Response<Author> result = await service.DeleteAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.DatabaseError);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An error occurred while deleting the author");

        _authorRepositoryMock.Verify(repo => repo.Delete(author), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        VerifyErrorLog("Error occurred while deleting author");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnError_WhenUnexpectedErrorOccurs()
    {
        // Arrange  
        AuthorService service = CreateService();
        Author author = TestDataHelper.Authors[0];
        _authorRepositoryMock.Setup(repo => repo.FindByIdAsync(author.Id)).ReturnsAsync(author);
        _authorRepositoryMock.Setup(repo => repo.Delete(author)).Throws(new Exception("Unexpected error"));

        // Act
        Response<Author> result = await service.DeleteAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Model.Should().BeNull();
        result.Error.Should().Be(ErrorType.Unknown);
        result.Message.Should().NotBeNull();
        result.Message.Should().Be("An unexpected error occurred while processing your request");

        _authorRepositoryMock.Verify(repo => repo.Delete(author), Times.Once);
        _authorRepositoryMock.Verify(repo => repo.FindByIdAsync(author.Id), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        VerifyErrorLog("Unexpected error occurred while deleting author");
    }
}