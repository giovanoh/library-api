using System.ComponentModel;

using FluentAssertions;

using Library.API.Extensions;

namespace Library.API.Tests.Extensions;

public enum TestEnumWithDescription
{
    [Description("First Value")]
    First,
    [Description("Second Value")]
    Second,
    [Description("Third Value")]
    Third
}

public enum TestEnumWithoutDescription
{
    First,
    Second,
    Third
}

public class EnumExtensionsTests
{
    [Fact]
    public void ToDescription_ShouldReturnDescriptionAttribute_WhenEnumHasDescription()
    {
        // Arrange
        var firstDescription = TestEnumWithDescription.First.ToDescription();
        var secondDescription = TestEnumWithDescription.Second.ToDescription();
        var thirdDescription = TestEnumWithDescription.Third.ToDescription();

        // Assert
        firstDescription.Should().Be("First Value");
        secondDescription.Should().Be("Second Value");
        thirdDescription.Should().Be("Third Value");
    }

    [Fact]
    public void ToDescription_ShouldReturnEnumName_WhenEnumHasNoDescription()
    {
        // Arrange
        var firstDescription = TestEnumWithoutDescription.First.ToDescription();
        var secondDescription = TestEnumWithoutDescription.Second.ToDescription();
        var thirdDescription = TestEnumWithoutDescription.Third.ToDescription();

        // Assert
        firstDescription.Should().Be("First");
        secondDescription.Should().Be("Second");
        thirdDescription.Should().Be("Third");
    }
}

