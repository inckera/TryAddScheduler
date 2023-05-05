using System.Runtime.InteropServices;
using Scheduler.Model;
using Scheduler.Presentation;
using Scheduler.Repository;
using StructureMap.Diagnostics;
using StreamWriter = System.IO.StreamWriter;

namespace Scheduler.Infrastructure.MeetingServices;

public class MeetingService : IMeetingCommands
{
    /// <summary>
    /// Репозиторий встреч.
    /// </summary>
    private readonly IMeetingRepository _meetingRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="meetingRepository">Репозиторий встреч.</param>
    public MeetingService(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public IEnumerable<Meeting> GetMeetings()
    {
        return _meetingRepository.GetMeetings();
    }

    public Tuple<bool, string> ImportMeetings()
    {
        try
        {
            var meetings = _meetingRepository.GetMeetings();
            var path = $"meetingsOn{DateTime.Now}.txt";
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var meeting in meetings)
                {
                    writer.WriteLine($"С {meeting.Begin} по {meeting.End} встреча {meeting.MeetingHeader}. Система предупредит Вас в {meeting.NotificationDate}");
                }
            }
            return new Tuple<bool, string>(true, null);
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, ex.Message);
        }
    }

    public Tuple<ulong, string> AddMeeting(Meeting meeting)
    {
        return _meetingRepository.AddMeeting(meeting);
    }

    public Tuple<bool, string> UpdateMeeting(Meeting meeting)
    {
        return _meetingRepository.UpdateMeeting(meeting);
    }

    public Tuple<bool, string> DeleteMeeting(ulong meetingId)
    {
        return _meetingRepository.DeleteMeeting(meetingId);
    }
}