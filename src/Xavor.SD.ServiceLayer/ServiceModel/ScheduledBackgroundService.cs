using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace Xavor.SD.ServiceLayer.ServiceModel
{
    [DisallowConcurrentExecution]
    public class ScheduledBackgroundService : IJob
    {

        private readonly ILogger<ScheduledBackgroundService> _logger;
        public ScheduledBackgroundService(ILogger<ScheduledBackgroundService> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Hello world!");
            return Task.CompletedTask;
        }
    }
}
