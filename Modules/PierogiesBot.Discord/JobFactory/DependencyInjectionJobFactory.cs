using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace PierogiesBot.Discord.JobFactory
{
    public class DependencyInjectionJobFactory : PropertySettingJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var job = _serviceProvider.GetService(bundle.JobDetail.JobType);
            if (ReferenceEquals(job, null))
                return base.NewJob(bundle, scheduler);
            SetObjectProperties(job, bundle.JobDetail.JobDataMap);
            return (IJob) job;
        }
    }
}