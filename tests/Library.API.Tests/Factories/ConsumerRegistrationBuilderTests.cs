using FluentAssertions;
using MassTransit;
using Moq;

using Library.API.Infrastructure.Factories;

namespace Library.API.Tests.Factories;

public class ConsumerRegistrationBuilderTests
{
    [Fact]
    public void Add_ShouldSetSuffix_WhenConsumerIsRegistered()
    {
        // Arrange
        var expectedSuffix = "test-suffix";
        var _configuratorMock = new Mock<IRegistrationConfigurator>();
        var _builder = new ConsumerRegistrationBuilder(_configuratorMock.Object, expectedSuffix);

        // Act
        _builder.Add<TestConsumer>();

        // Assert
        SuffixedConsumerDefinition<TestConsumer>.CurrentSuffix.Should().Be(expectedSuffix);
    }

    // Test consumer class for our tests
    private class TestConsumer : IConsumer<TestMessage>
    {
        public Task Consume(ConsumeContext<TestMessage> context)
        {
            return Task.CompletedTask;
        }
    }

    // Test message class for our consumer
    private class TestMessage
    {
    }
}