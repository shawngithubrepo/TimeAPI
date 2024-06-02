using Moq;
using Time.Services.TimeAPI.Services.IService;
using Microsoft.Extensions.Logging;

namespace TimeAPI.Tests.ServicesTests
{
    [TestClass]
    public class TimeServiceTests
    {
        private Mock<ILogger<TimeService>> _loggerMock;
        private ITimeService _timeService;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<TimeService>>();
            _timeService = new TimeService(_loggerMock.Object);
        }

        [DataTestMethod]
        [DataRow("+05:30", "2022-01-01T00:00:00Z", "2022-01-01T05:30:00")]
        [DataRow("-07:00", "2022-01-01T00:00:00Z", "2021-12-31T17:00:00")]
        public async Task GetAdjustedTime_ValidTimezone_ShouldReturnAdjustedTime(string timezone, string currentTime, string expectedTime)
        {
            // Arrange
            DateTime curr = DateTime.Parse(currentTime).ToUniversalTime();
            DateTime expected = DateTime.Parse(expectedTime);

            // Act
            var result = await _timeService.GetAdjustedTimeAsync(timezone, curr);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("InvalidTimezone")]
        [DataRow("+25:00")]
        public async Task GetAdjustedTime_InvalidTimezone_ShouldThrowArgumentException(string timezone)
        {
            // Arrange
            DateTime curr = DateTime.UtcNow;

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _timeService.GetAdjustedTimeAsync(timezone, curr));
        }
    }
}
