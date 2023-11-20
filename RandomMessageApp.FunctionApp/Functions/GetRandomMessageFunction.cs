using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RandomMessageApp.FunctionApp.Services.Interfaces;

namespace RandomMessageApp.FunctionApp.Functions
{
    public class GetRandomMessageFunction
    {
        private readonly IRandomMessageService _randomMessageService;

        public GetRandomMessageFunction(IRandomMessageService randomMessageService)
        {
            _randomMessageService = randomMessageService;
        }

        [Singleton]
        [FunctionName(nameof(GetRandomMessage))]
        public async Task GetRandomMessage(
            [TimerTrigger("%ScheduleConfiguration:GetRandomMessageFunction:CronExpression%")] TimerInfo myTimer,
            ILogger log)
        {
            await _randomMessageService.RunAsync();
        }
    }
}
