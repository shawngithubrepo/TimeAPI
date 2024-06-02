    using Microsoft.Extensions.Logging;
    using Moq;
    using Time.Services.TimeAPI.Controllers;
    using Time.Services.TimeAPI.Models.Dto;
    using Microsoft.AspNetCore.Mvc;

namespace Time.Services.TimeAPI.Tests.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        private TimeController _timeController;
        private TimeService _timeService;
        private Mock<ILogger<TimeService>> _timeServiceLoggerMock;
        private Mock<ILogger<TimeController>> _controllerLoggerMock;

        [TestInitialize]
        public void Setup()
        {
            _timeServiceLoggerMock = new Mock<ILogger<TimeService>>();
            _controllerLoggerMock = new Mock<ILogger<TimeController>>();
            _timeService = new TimeService(_timeServiceLoggerMock.Object);
            _timeController = new TimeController(_timeService, _controllerLoggerMock.Object);
        }

        [TestMethod]
        public async Task Get_WithValidTimezone_ReturnsAdjustedTime()
        {
            // Arrange
            string timezone = "+05:30";

            // Act
            IActionResult actionResult = await _timeController.GetAsync(timezone);
            OkObjectResult okResult = actionResult as OkObjectResult;
            

            // Act and Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            ResponseDto response = okResult.Value as ResponseDto;
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccess);

            TimeDto timeDto = response.Result as TimeDto;
            Assert.IsNotNull(timeDto);
            Assert.IsNotNull(timeDto.AdjustedTime);
        }

        [TestMethod]
        public async Task Get_WithInvalidTimezone_ReturnsBadRequest()
        {
            // Arrange
            string timezone = "Invalid";

            // Act
            IActionResult actionResult = await _timeController.GetAsync(timezone);
            BadRequestObjectResult badRequestResult = actionResult as BadRequestObjectResult ?? throw new AssertFailedException("Expected BadRequest");

            // Assert
            Assert.AreEqual(400, badRequestResult.StatusCode);

            ResponseDto response = badRequestResult.Value as ResponseDto ?? throw new AssertFailedException("Expected BadRequestObjectResult for ResponseDto.");
            Assert.IsFalse(response.IsSuccess);
        }
    }
}
