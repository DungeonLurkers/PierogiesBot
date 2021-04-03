using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using NCrontab;

namespace Module.Core.Extensions
{
    public static class CronObservable
    {
        public static string Jp2CronTab = "37 21 * * *";
        public static string BlazeCronTab = "20 16 * * *";
        public static string Blaze2CronTab = "20 4 * * *";

        public static TimeZoneInfo Timezone { get; }

        static CronObservable()
        {
            Timezone = TimeZoneInfo.FromSerializedString("Central European Standard Time;60;(UTC+01:00) Sarajewo, Skopie, Warszawa, Zagrzeb;�rodkowoeuropejski czas stand.;�rodkowoeuropejski czas letni;[01:01:0001;12:31:9999;60;[0;02:00:00;3;5;0;];[0;03:00:00;10;5;0;];];");
        }
        public static IObservable<int> Cron(string cron, IScheduler scheduler)
        {

            var schedule = CrontabSchedule.Parse(cron);
            return Observable.Generate(0, d => true, d => d + 1, d => d,
                d => new DateTimeOffset(schedule.GetNextOccurrence(TimeZoneInfo.ConvertTimeFromUtc(scheduler.Now.UtcDateTime, Timezone))));
        }
    }
}
