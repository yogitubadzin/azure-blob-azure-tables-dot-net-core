using RandomMessageApp.Core.CommonServices.Interfaces;

namespace RandomMessageApp.Core.CommonServices.Implementation;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow { get; } = DateTime.UtcNow;
}
