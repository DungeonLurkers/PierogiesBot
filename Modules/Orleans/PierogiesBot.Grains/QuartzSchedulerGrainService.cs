using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using PierogiesBot.GrainsInterfaces;
using Quartz;

namespace PierogiesBot.Grains
{
    public class QuartzSchedulerGrainService : GrainService, IQuartzSchedulerGrainService
    {
        private IScheduler _scheduler;

        public QuartzSchedulerGrainService(IGrainIdentity id, Silo silo, ILoggerFactory loggerFactory) : base(id, silo, loggerFactory)
        {
            
        }

        public override Task Init(IServiceProvider serviceProvider)
        {
            _scheduler = serviceProvider.GetRequiredService<IScheduler>();
            return base.Init(serviceProvider);
        }

        public override async Task Start()
        {
            await _scheduler.Start();
            await base.Start();
        }

        public override async Task Stop()
        {
            await _scheduler.Shutdown();
            await base.Stop();
        }
    }
}