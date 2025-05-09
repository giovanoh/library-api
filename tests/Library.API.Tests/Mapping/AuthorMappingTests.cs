using FluentAssertions;

using Library.API.Domain.Models;
using Library.API.DTOs;

namespace Library.API.Tests.Mapping;

public class AuthorMappingTests : MappingTestsBase
{
    [Fact]
    public void Should_Map_Author_To_AuthorDto()
    {
        // Arrange
        var author = new Author
        {
            Id = 1,
            Name = "Author 1",
            BirthDate = new DateTime(1980, 1, 1),
            Biography = "Author 1 is a software engineer."
        };

        // Act
        var result = Mapper.Map<AuthorDto>(author);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(author.Id);
        result.Name.Should().Be(author.Name);
        result.BirthDate.Should().Be(author.BirthDate);
        result.Biography.Should().Be(author.Biography);
    }

    [Fact]
    public void Should_Map_SaveAuthorDto_To_Author()
    {
        // Arrange
        var authorDto = new SaveAuthorDto
        {
            Name = "Author 1",
            BirthDate = new DateTime(1980, 1, 1),
            Biography = "Author 1 is a software engineer."
        };

        // Act
        var result = Mapper.Map<Author>(authorDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(authorDto.Name);
        result.BirthDate.Should().Be(authorDto.BirthDate);
        result.Biography.Should().Be(authorDto.Biography);
    }
}