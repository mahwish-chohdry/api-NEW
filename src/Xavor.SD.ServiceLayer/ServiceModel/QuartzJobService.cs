using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;


namespace Xavor.SD.ServiceLayer.ServiceModel
{
    public class QuartzJobService : IJobFactory

    {
        private readonly IServiceProvider _serviceProvider;
        public QuartzJobService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job) { }
    }
}
