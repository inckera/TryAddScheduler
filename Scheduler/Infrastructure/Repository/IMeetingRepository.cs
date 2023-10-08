using Scheduler.Model;

namespace Scheduler.Repository;

/// <summary>
/// Репозиторий для встреч.
/// </summary>
public interface IMeetingRepository : IDisposable
{
    /// <summary>
    /// Получить список встреч.
    /// </summary>
    /// <returns>Список встреч.</returns>
    IEnumerable<Meeting> GetMeetings();

    /// <summary>
    /// Получить список встреч на дату.
    /// </summary>
    /// <param name="notificationDate">Дата уведомления.</param>
    /// <returns>Список встреч.</returns>
    IEnumerable<Meeting> GetMeetingsByDate(DateTime notificationDate);

    /// <summary>
    /// Добавить встречу.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <returns>Идентификатор встречи, сообщение об ошибке.</returns>
    Tuple<ulong, string> AddMeeting(Meeting meeting);

    /// <summary>
    /// Обновить встречу.
    /// </summary>
    /// <param name="meeting">Встреча.</param>
    /// <returns>Успешность, сообщение об ошибке.</returns>
    Tuple<bool, string> UpdateMeeting(Meeting meeting);

    /// <summary>
    /// Удалить встречу.
    /// </summary>
    /// <param name="MeetingId">Идентификатор встречи.</param>
    /// <returns>Успешность, сообщение об ошибке.</returns>
    Tuple<bool, string> DeleteMeeting(ulong meetingId);
}