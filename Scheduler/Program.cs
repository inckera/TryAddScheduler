// See https://aka.ms/new-console-template for more information

using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Scheduler;
using Scheduler.Infrastructure;
using Scheduler.Infrastructure.MeetingServices;
using Scheduler.Presentation;
using Scheduler.Repository;


#region Табло

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("===============================================");
Console.WriteLine("Тут будет расписание");
Console.WriteLine("===============================================");
Console.SetCursorPosition(0,5);
Console.ResetColor();
#endregion

#region Расписание.
// construct a scheduler factory
StdSchedulerFactory factory = new StdSchedulerFactory(new NameValueCollection
{
    { "quartz.serializer.type", "binary" }
});

// get a scheduler
IScheduler scheduler = await factory.GetScheduler();
await scheduler.Start();

// define the job and tie it to our HelloJob class
IJobDetail job = JobBuilder.Create<MeetingNotificationJob>()
    .WithIdentity("meetingJob", "group1")
    .Build();

// Trigger the job to run now, and then every 40 seconds
ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("myTrigger", "group1")
    .StartNow()
    .WithSimpleSchedule(x => x
      //  .WithIntervalInMinutes(1)
        .WithIntervalInSeconds(15)
        .RepeatForever())
    
    .Build();

//Thread scheduleThread = new Thread(ScheduleStart);

//scheduleThread.Start();
//Запуск
var t = await scheduler.ScheduleJob(job, trigger);
//Console.ReadKey();

#endregion

#region Встречи

var serviceProvider = new ServiceCollection()
    .AddSingleton<IMeetingCommands, MeetingService>()
    .AddSingleton<IMeetingRepository, MeetingMemoryRepository>()
    .AddSingleton<MeetingWorker>()
    .AddSingleton<MeetingValidator>()
    .BuildServiceProvider();

//Thread readerThread = new Thread(StartRead);
//readerThread.Start();

var worker = serviceProvider.GetService<MeetingWorker>();
Task task =   Task.Factory.StartNew(worker.Start);
Task.WaitAny(new Task[]{task}, 15000 );




#endregion