using Quartz;
using Scheduler.Repository;

namespace Scheduler;

/// <summary>
/// Уведомление о встрече.
/// </summary>
public class MeetingNotificationJob : IJob
{
    private IMeetingRepository _meetingRepository;

    public MeetingNotificationJob(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    /// <summary>
    /// Событие при срабатывании уведомления.
    /// </summary>
    /// <param name="context">Контекст.</param>
    public async Task Execute(IJobExecutionContext context)
    {
        var cursorPosition = Console.GetCursorPosition();

        Console.SetCursorPosition(2, 2);
        Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
        var meetings = _meetingRepository.GetMeetings();
        var meetForStart = meetings.FirstOrDefault((m => m.NotificationDate <= DateTime.Now));
        if (meetForStart != null)
        {
            await SendConsoleMessage(
                $" Встреча {meetForStart.MeetingHeader} с {meetForStart.Begin} до {meetForStart.End}");
            try
            {
                _meetingRepository.DeleteMeeting(meetForStart.MeetingId);
            }
            catch
            {
                //Заглушка
            }
        }

        Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
    }

    /// <summary>
    /// Вывод сообщения на консоль.
    /// </summary>
    /// <param name="message">Сообщение для вывода.</param>
    private async Task SendConsoleMessage(string message) => await Console.Out.WriteLineAsync(message);
}