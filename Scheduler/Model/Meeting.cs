namespace Scheduler.Model;

/// <summary>
/// Встреча.
/// </summary>
public class Meeting
{
    /// <summary>
    /// Идентификатор встречи.
    /// </summary>
    public ulong MeetingId { get; set; }

    /// <summary>
    /// Дата сообщения о встрече.
    /// </summary>
    public DateTime NotificationDate { get; private set; }

    /// <summary>
    /// Заголовок встречи.
    /// </summary>
    public string MeetingHeader { get; set; }

    /// <summary>
    /// Начало встречи.
    /// </summary>
    public DateTime Begin { get; set; }

    /// <summary>
    /// Окончание встречи.
    /// </summary>
    public DateTime End { get; set; }

    private int _minutesForCall;

    public int MinutesForCall
    {
        get => _minutesForCall;
        set
        {
            _minutesForCall = value;
            NotificationDate = Begin.AddMinutes(-value);
        }
    }
}