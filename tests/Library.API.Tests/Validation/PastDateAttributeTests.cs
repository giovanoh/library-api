using FluentAssertions;

using Library.API.Validation;

namespace Library.API.Tests.Validation;

public class PastDateAttributeTests
{
    [Fact]
    public void IsValid_ShouldReturnTrue_WhenDateIsPast()
    {
        // Arrange
        var attribute = new PastDateAttribute();
        var date = DateTime.Now.AddDays(-1);

        // Act
        var result = attribute.IsValid(date);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenDateIsFuture()
    {
        // Arrange
        var attribute = new PastDateAttribute();
        var date = DateTime.Now.AddDays(1);

        // Act
        var result = attribute.IsValid(date);

        // Assert       
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenDateIsNull()
    {
        // Arrange
        var attribute = new PastDateAttribute();
        DateTime? date = null;

        // Act
        var result = attribute.IsValid(date);

        // Assert
        result.Should().BeTrue();
    }
    
    
}