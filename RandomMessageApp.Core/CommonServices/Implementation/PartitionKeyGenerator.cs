using RandomMessageApp.Core.CommonServices.Interfaces;

namespace RandomMessageApp.Core.CommonServices.Implementation;

public class PartitionKeyGenerator : IPartitionKeyGenerator
{
    public string Generate(DateTime dateTime)
    {
        var roundHour = dateTime.Date.AddHours(dateTime.Hour);
        return (DateTime.MaxValue - roundHour.ToUniversalTime()).Ticks.ToString();
    }
}
