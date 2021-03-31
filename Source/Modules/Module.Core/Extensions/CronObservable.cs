using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NCrontab;

namespace Module.Core.Extensions
{
    public static class CronObservable
    {
        public static string Jp2CronTab = "37 21 * * *";
        public static string BlazeCronTab = "20 14 * * *";
        public static string Blaze2CronTab = "20 4 * * *";
        public static IObservable<int> Cron(string cron, IScheduler scheduler)
        {
            var schedule = CrontabSchedule.Parse(cron);
            return Observable.Generate(0, d => true, d => d + 1, d => d,
                d => new DateTimeOffset(schedule.GetNextOccurrence(scheduler.Now.UtcDateTime.ToLocalTime())), scheduler);
        }
    }
}
