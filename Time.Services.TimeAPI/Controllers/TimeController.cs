using Microsoft.AspNetCore.Mvc;
using Time.Services.TimeAPI.Models.Dto;
using Time.Services.TimeAPI.Services.IService;

namespace Time.Services.TimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        private readonly ITimeService _timeService;
        private readonly ILogger<TimeController> _logger;
        private readonly ResponseDto _response;

        public TimeController(ITimeService timeService, ILogger<TimeController> logger)
        {
            _timeService = timeService;
            _logger = logger;
            _response = new ResponseDto();
        }

        /// <summary>
        /// Gets the current UTC time and optionally the adjusted time for given timezone.
        /// </summary>
        /// <param name="timezone">A optional string representing a timezone offset.</param>
        /// <returns>The current UTC time and optionally the adjusted time for the specified timezone if its not null.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string timezone = null)
        {
            _logger.LogInformation("Received request for current time with timezone: {Timezone}", timezone);

            try
            {
                var utcNow = DateTime.UtcNow;                
                _response.Result = new TimeDto
                {
                    CurrentTime = utcNow,
                    AdjustedTime = timezone != null ? await _timeService.GetAdjustedTimeAsync(timezone, utcNow) : (DateTime?)null
                };
                _response.IsSuccess = true;
                _logger.LogInformation("Returning time information with timezone: {Timezone}", timezone);
            }
            catch (ArgumentException ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                _logger.LogWarning(ex, "Invalid timezone format: {Timezone}", timezone);
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "An error occurred while processing your request.";
                _logger.LogError(ex, "An error occurred while processing the time request.");
                return StatusCode(500, _response);
            }

            return Ok(_response);
        }
    }
}
