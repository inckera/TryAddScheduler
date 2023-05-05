using System;
using System.Threading.Tasks;
using static System.Console;

namespace Scheduler.Presentation;

public static class ConsoleHelpers
{
    public static string ConsoleReadLineWithTimeout(TimeSpan? timeout = null)
    {

        var timeSpan = timeout ?? TimeSpan.FromSeconds(1);
        var task = Task.Factory.StartNew(ReadLine);
        var result = (Task.WaitAny(new Task[] { task }, timeSpan) == 0) ? task.Result : string.Empty;

        return result;

    }

    public static string ReadLineWaitFiveSecond() => ConsoleReadLineWithTimeout(TimeSpan.FromSeconds(5));
    public static string ReadLineWaitTenSecond() => ConsoleReadLineWithTimeout(TimeSpan.FromSeconds(10));
}