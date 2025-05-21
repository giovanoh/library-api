using Microsoft.Extensions.Logging;

using Moq;

namespace Library.API.Tests.Services;

public class ServiceTestBase<TService>
{
    protected readonly Mock<ILogger<TService>> LoggerMock = new();

    protected void VerifyErrorLog(string expectedMessage)
    {
        LoggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v!.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
}
