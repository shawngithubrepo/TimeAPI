using NodaTime;
using NodaTime.Text;
using Time.Services.TimeAPI.Services.IService;

public class TimeService : ITimeService
{
    private readonly ILogger<TimeService> _logger;

    public TimeService(ILogger<TimeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the adjusted time .
    /// </summary>
    /// <param name="timezone">A string representing a offset.</param>
    /// <param name="curr">The current UTC time.</param>
    /// <returns>adjusted time.</returns>
    public async Task<DateTime?> GetAdjustedTimeAsync(string timezone, DateTime curr)
    {
        try
        {
            timezone = PreprocessTimezone(timezone);

            var parseResult = OffsetPattern.GeneralInvariant.Parse(timezone);

            if (parseResult.Success)
            {
                var offset = parseResult.Value;
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(curr, DateTimeKind.Utc));
                var adjustedTime = instant.WithOffset(offset).ToDateTimeOffset().DateTime;
                return await Task.FromResult(adjustedTime);
            }
            else
            {
                _logger.LogWarning("Failed to parse timezone offset: {Timezone}", timezone);
                throw new ArgumentException($"Invalid timezone format: {timezone}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adjusting time with timezone: {Timezone}", timezone);
            throw;
        }
    }

    /// <summary>
    /// this handles some of the invalid input format and correct it, doesn't correct every invalid input , but this is good enough for our demo
    /// </summary>
    /// <param name="timezone">timezone string.</param>
    /// <returns>processed PreprocessTimezone.</returns>
    /// <exception cref="ArgumentException">Thrown when the timezone format is invalid.</exception>
    private string PreprocessTimezone(string timezone)
    {
        timezone = timezone.Trim();

        if (timezone[0] != '+' && timezone[0] != '-')
        {
            throw new ArgumentException("Timezone must start with '+' or '-'", nameof(timezone));
        }

        if (timezone.Any(x => char.IsLetter(x)))
        {
            throw new ArgumentException("Timezone must not contain alphabetic characters", nameof(timezone));
        }

        // correcting +5 or -5 
        if (timezone.Length == 2)
        {
            timezone += ":00";
        }
        // correcting  +05 or -15
        else if (timezone.Length == 3)
        {
            timezone += ":00";
        }

        //correcting  +5:30 or -5:00 and into +05:30 or -05:00
        if (timezone.Length == 5 && timezone[2] == ':')
        {
            timezone = timezone.Insert(1, "0");
        }
        // correcting  like +530 or -500  and into +5:30 or -5:00
        else if (timezone.Length == 4 && char.IsDigit(timezone[2]) && char.IsDigit(timezone[3]))
        {
            timezone = timezone.Insert(2, ":");
        }
        // Handle cases like +0530 or -0500
        else if (timezone.Length == 5 && char.IsDigit(timezone[3]) && char.IsDigit(timezone[4]))
        {
            timezone = timezone.Insert(3, ":");
        }
        // Handle cases like +5:30 or -5:00
        else if (timezone.Length == 6 && timezone[3] == ':')
        {
            //covers good enough format correction by now
        }
        else
        {
            throw new ArgumentException("Invalid timezone format", nameof(timezone));
        }

        return timezone;
    }

}
