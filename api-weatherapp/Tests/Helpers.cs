using Microsoft.Extensions.Logging;
using Moq;

namespace Tests;

public class Helpers
{
    public static ILogger<T> GetLoggerMock<T>() where T : class
    {
        return new Mock<ILogger<T>>().Object;
    } 
}
