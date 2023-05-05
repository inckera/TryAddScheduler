using Scheduler.Model;

namespace Scheduler.Repository;

/// <summary>
/// Репозиторий для встреч (хранение в памяти).
/// </summary>
public class MeetingMemoryRepository : IMeetingRepository
{
    /// <summary>
    /// Список встреч.
    /// </summary>
    protected List<Meeting> meetingsList = new List<Meeting>();

    /// <summary>
    /// Счётчик для присвоения следующего идентификатора.
    /// </summary>
    private ulong nextId = 1;

    public IEnumerable<Meeting> GetMeetings()
    {
        return meetingsList;
    }

    public IEnumerable<Meeting> GetMeetingsByDate(DateTime notificationDate)
    {
        return meetingsList.Where(m => m.NotificationDate.Date == notificationDate.Date);
    }

    public Tuple<ulong, string> AddMeeting(Meeting meeting)
    {
        meeting.MeetingId = nextId++;
        try
        {
            meetingsList.Add(meeting);
            return new Tuple<ulong, string>(meeting.MeetingId, null);
        }
        catch (Exception ex)
        {
            return new Tuple<ulong, string>(0, ex.Message);
        }
    }

    public Tuple<bool, string> UpdateMeeting(Meeting meeting)
    {
        var meetingFromList = meetingsList.FirstOrDefault(m => m.MeetingId == meeting.MeetingId);
        if (meetingFromList != null)
        {
            meetingFromList = meeting;
            return new Tuple<bool, string>(true, null);
        }
        else
        {
            return new Tuple<bool, string>(false, "Данный элемент не найден");
        }
    }

    public Tuple<bool, string> DeleteMeeting(ulong meetingId)
    {
        var meetingFromList = meetingsList.FirstOrDefault(m => m.MeetingId == meetingId);
        if (meetingFromList == null)
            return new Tuple<bool, string>(false, "Встреча не найдена в списке");
        try
        {
            meetingsList.Remove(meetingFromList);
            return new Tuple<bool, string>(true, null);
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, ex.Message);
        }
    }

    /// <summary>
    /// Очистить.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}