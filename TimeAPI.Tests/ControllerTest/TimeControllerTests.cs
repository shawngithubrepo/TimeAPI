using Microsoft.Extensions.Logging;
using Moq;
using Time.Services.TimeAPI.Controllers;
using Time.Services.TimeAPI.Models.Dto;
using Time.Services.TimeAPI.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace Time.Services.TimeAPI.Tests.Controllers
{
    [TestClass]
    public class TimeControllerTests
    {
        private Mock<ITimeService> _timeServiceMock;
        private Mock<ILogger<TimeController>> _loggerMock;
        private TimeController _timeController;

        [TestInitialize]
        public void Setup()
        {
            _timeServiceMock = new Mock<ITimeService>();
            _loggerMock = new Mock<ILogger<TimeController>>();
            _timeController = new TimeController(_timeServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Get_WithValidTimezone_ReturnsAdjustedTime()
        {
            // Arrange
            string timezone = "+05:30";
            DateTime currentTimeUtc = DateTime.UtcNow;
            DateTime adjustedTime = currentTimeUtc.AddHours(5.5);
            _timeServiceMock.Setup(service => service.GetAdjustedTimeAsync(timezone, It.IsAny<DateTime>()))
                            .ReturnsAsync(adjustedTime);

            // Act
            IActionResult actionResult = await _timeController.GetAsync(timezone);
            OkObjectResult okResult = actionResult as OkObjectResult ?? throw new AssertFailedException("Expected OkObjectResult");

            // Assert
            Assert.AreEqual(200, okResult.StatusCode);

            ResponseDto response = okResult.Value as ResponseDto ?? throw new AssertFailedException("Expected OkObjectResult.Value in ResponseDto");
            Assert.IsTrue(response.IsSuccess);

            TimeDto timeDto = response.Result as TimeDto ?? throw new AssertFailedException("Expected ResponseDto.Result TimeDto format");
            Assert.IsNotNull(timeDto.AdjustedTime);
            Assert.AreEqual(timeDto.AdjustedTime, adjustedTime);
        }

        [TestMethod]
        public async Task Get_WithInvalidTimezone_ReturnsBadRequest()
        {
            // Arrange
            var timezone = "Invalid";
            _timeServiceMock.Setup(x => x.GetAdjustedTimeAsync(It.IsAny<string>(), It.IsAny<DateTime>())).Throws(new ArgumentException());

            // Act
            var result = await _timeController.GetAsync(timezone) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);

            var response = result.Value as ResponseDto ?? throw new AssertFailedException("Expected BadRequestObjectResult.Value to be of type ResponseDto");
            Assert.IsFalse(response.IsSuccess);
        }
    }
}
