using FluentAssertions;

using Library.API.Domain.Models;
using Library.API.DTOs;

namespace Library.API.Tests.Mapping;

public class BookMappingTests : MappingTestsBase
{
    [Fact]
    public void Should_Map_Book_To_BookDto()
    {
        // Arrange
        var book = new Book
        {
            Id = 1,
            Title = "Book 1",
            Description = "Book 1 is a science fiction book"
        };

        // Act
        var result = Mapper.Map<BookDto>(book);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(book.Id);
        result.Title.Should().Be(book.Title);
        result.Description.Should().Be(book.Description);   
    }

    [Fact]
    public void Should_Map_SaveBookDto_To_Book()
    {
        // Arrange
        var bookDto = new SaveBookDto
        {
            Title = "Book 1",
            Description = "Book 1 is a science fiction book"
        };

        // Act
        var result = Mapper.Map<Book>(bookDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(bookDto.Title);
        result.Description.Should().Be(bookDto.Description);
    }
}