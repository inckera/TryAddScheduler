using Scheduler.Model;
using Scheduler.Presentation;
using Scheduler.Repository;

namespace Scheduler.Infrastructure;

/// <summary>
/// Do not use
/// </summary>
public class MeetingValidator
{
    private readonly IMeetingRepository _meetingRepository;

    private const int MINUTES_FOR_CHECK = 2;
    
    public MeetingValidator(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public List<string> ValidateMeeting(Meeting meeting)
    {
        var remarks = new List<string>();
        var addTimeForCheck = DateTime.Now.AddMinutes(MINUTES_FOR_CHECK);
        if (meeting.Begin <= addTimeForCheck)
            remarks.Add("Добавьте встречу с более поздней датой начала");
        if (meeting.End <= addTimeForCheck)
            remarks.Add("Добавьте встречу с более поздней датой окончания");
        if (meeting.NotificationDate <= addTimeForCheck)
            remarks.Add("Добавьте встречу с более поздней датой начала или меньшим промежутком до напоминания");
        if (meeting.Begin >= meeting.End)
            remarks.Add("Начало встречи должно быть раньше окончания");
        var _meetings = _meetingRepository.GetMeetings();
        var intersectStartMeeting = _meetings.FirstOrDefault(m => m.Begin <= meeting.Begin && m.End >= meeting.Begin);
        if (intersectStartMeeting != null)
            remarks.Add($"C {intersectStartMeeting.Begin} по {intersectStartMeeting.End} уже есть встреча {intersectStartMeeting.MeetingHeader}");
        var intersectEndMeeting = _meetings.FirstOrDefault(m => m.Begin <= meeting.End && m.End >= meeting.End);
        if (intersectStartMeeting != null)
            remarks.Add($"C {intersectEndMeeting.Begin} по {intersectEndMeeting.End} уже есть встреча {intersectEndMeeting.MeetingHeader}");
        return remarks;
    }
}