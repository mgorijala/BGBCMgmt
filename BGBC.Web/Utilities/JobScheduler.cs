using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BGBC.Web.Utilities
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = Quartz.Impl.StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInSeconds(120)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);

        }
    }
}