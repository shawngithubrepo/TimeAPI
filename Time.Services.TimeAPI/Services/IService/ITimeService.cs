namespace Time.Services.TimeAPI.Services.IService
{
    public interface ITimeService
    {
        Task<DateTime?> GetAdjustedTimeAsync(string timezone, DateTime curr);
    }
}
