using System.Diagnostics;
using Scheduler.Model;

namespace Scheduler.Presentation;

public class MeetingWorker
{
    /// <summary>
    /// Команды.
    /// </summary>
    private readonly IMeetingCommands _meetingCommands;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="meetingCommands">Команды.</param>
    public MeetingWorker(IMeetingCommands meetingCommands)
    {
        _meetingCommands = meetingCommands;
    }

    /// <summary>
    /// Начало работы.
    /// </summary>
    public async Task Start()
    {
        while (true)
        {
            
            var commandExspression = await Console.In.ReadLineAsync();
            if (commandExspression == null)
            {
                Console.WriteLine("Ошибка!");
                return;
            }

            //Справка.
            if (commandExspression.Length >= 4 && commandExspression.ToLower().Substring(0, 4) == "help")
                WriteCommandsList();
            // Выгрузка txt.
            else if (commandExspression.Length >= 6 && commandExspression.ToLower().Substring(0, 6) == "import")
                ImportToTxt();
            //Добавить
            else if (commandExspression.Length >= 3 && commandExspression.ToLower().Substring(0, 3) == "add")
                AddMeetingDialog();
            //Изменить.
            else if (commandExspression.Length >= 6 && commandExspression.ToLower().Substring(0, 6) == "update")
                UpdateMeeting();
            //Удалить.
            else if (commandExspression.Length >= 6 && commandExspression.ToLower().Substring(0, 6) == "delete")
                DeleteMeeting();
            else
                Console.WriteLine("Команда не распознана");
        }
    }

    /// <summary>
    /// Вывод списка доступных команд.
    /// </summary>
    private void WriteCommandsList()
    {
        Console.WriteLine("Команда add добавит встречу;");
        Console.WriteLine("Команда update обновит встречу;");
        Console.WriteLine("Команда delete удалит встречу;");
        Console.WriteLine("Команда import сохранит список встреч в txt;");
        Console.WriteLine("Команда help выведет список этих команд;");
    }

    /// <summary>
    /// Удалить встречу.
    /// </summary>
    private void DeleteMeeting()
    {
        var meetingList = _meetingCommands.GetMeetings();
        meetingList.OrderBy(m => m.Begin);
        Console.WriteLine("Введите номер встречи, которую хотите удалить: ");
        foreach (var meeting in meetingList)
        {
            Console.WriteLine(
                $"{meeting.MeetingId}.С {meeting.Begin} по {meeting.End} встреча {meeting.MeetingHeader}. Оповещение {meeting.NotificationDate}");
        }

        var result = TryReadValue(DataTypeEnum.Integer, false);
        var id = (ulong)Convert.ToInt64(result.Item2);
        var meetingForUpdate = meetingList.FirstOrDefault(m => m.MeetingId == id);
        if (meetingForUpdate == null)
        {
            Console.WriteLine($"Встреча с номером {id} не найдена в списке.");
        }
        else
        {
            Console.WriteLine("Вы уверены, что хотите удалить встречу? Введите Да, чтобы продолжить");
            var answer = Console.ReadLine().ToLower();
            if (answer == "да")
            {
                _meetingCommands.DeleteMeeting(id);
            }
        }
    }

    /// <summary>
    /// Обновить встречу.
    /// </summary>
    private void UpdateMeeting()
    {
        var meetingList = _meetingCommands.GetMeetings();
        meetingList.OrderBy(m => m.Begin);
        Console.WriteLine("Введите номер встречи, которую хотите изменить: ");
        foreach (var meeting in meetingList)
        {
            Console.WriteLine(
                $"{meeting.MeetingId}.С {meeting.Begin} по {meeting.End} встреча {meeting.MeetingHeader}. Оповещение {meeting.NotificationDate}");
        }

        var result = TryReadValue(DataTypeEnum.Integer, false);
        var meetingForUpdate = meetingList.FirstOrDefault(m => m.MeetingId == (ulong)Convert.ToInt64(result.Item2));
        if (meetingForUpdate == null)
        {
            Console.WriteLine("Встреча с таким номером не найдена в списке.");
            return;
        }

        Console.WriteLine("Введите наименование: ");
        result = TryReadValue(DataTypeEnum.String, true);
        if (result.Item1)
            meetingForUpdate.MeetingHeader = result.Item2.ToString();
        Console.WriteLine("Введите время начала: ");
        result = TryReadValue(DataTypeEnum.DateTime, true);
        if (result.Item1)
            meetingForUpdate.Begin = (DateTime)result.Item2;
        Console.WriteLine("Введите время окончания : ");
        result = TryReadValue(DataTypeEnum.DateTime, true);
        if (result.Item1)
            meetingForUpdate.End = (DateTime)result.Item2;
        Console.WriteLine("За сколько минут до начала события предупредить? ");
        result = TryReadValue(DataTypeEnum.Integer, true);
        if (result.Item1)
            meetingForUpdate.MinutesForCall = (int)result.Item2;

        var updateResult = _meetingCommands.UpdateMeeting(meetingForUpdate);
        if (updateResult.Item1)
        {
            Console.WriteLine($"Встреча успешно обновлена");
        }
        else
            Console.WriteLine($"Не удалось добавить встречу. Причина: {result.Item2}");
    }

    /// <summary>
    /// Считывание встречи из консоли для добавления.
    /// </summary>
    private void AddMeetingDialog()
    {
        var meeting = new Meeting();
        Console.WriteLine("Введите наименование события: ");
        meeting.MeetingHeader = Console.ReadLine();
        meeting.Begin = TryReadDate("Начало события");
        meeting.End = TryReadDate("Конец события");
        Console.WriteLine("За сколько минут до начала события предупредить? ");
        var minutes = TryReadValue(DataTypeEnum.Integer);
        if (minutes.Item1)
            meeting.MinutesForCall = (int)minutes.Item2;

        var result = _meetingCommands.AddMeeting(meeting);
        if (string.IsNullOrEmpty(result.Item2))
        {
            Console.WriteLine($"Успешно добавлена встреча с идентификатором {result.Item1}");
        }
        else
            Console.WriteLine($"Не удалось добавить встречу. Причина: {result.Item2}");
    }

    /// <summary>
    /// Попытка считать дату из консоли.
    /// </summary>
    /// <param name="dateName">Пользовательское название даты.</param>
    /// <returns>Дата.</returns>
    private DateTime TryReadDate(string dateName)
    {
        Console.Write($"Введите дату поля {dateName}:");
        var dateString = Console.ReadLine();
        DateTime date = DateTime.Now;
        var successParse = false;
        while (!successParse)
        {
            successParse = DateTime.TryParse(dateString, out date);
            if (!successParse)
            {
                Console.Write($" Неверный формат! Ожидается дата в формате \"дд.мм.гггг чч:мм:сс\"");
                dateString = Console.ReadLine();
            }
        }

        return date;
    }

    /// <summary>
    /// Попытка считать значение из консоли.
    /// </summary>
    /// <param name="type">Тип данных.</param>
    /// <param name="canBeEmpty">Может ли вернуться пустое значение.</param>
    /// <returns>Значение не пусто, само значение.</returns>
    /// <exception cref="NotImplementedException">Для остальных типов.</exception>
    private Tuple<bool, object> TryReadValue(DataTypeEnum type, bool canBeEmpty = false)
    {
        var value = Console.ReadLine();
        if (canBeEmpty && string.IsNullOrEmpty(value))
            return new Tuple<bool, object>(false, null);
        switch (type)
        {
            case DataTypeEnum.String:
                while (string.IsNullOrEmpty(value))
                {
                    Console.WriteLine("Значение поля не может быть пустым. Введите значение:");
                    value = Console.ReadLine();
                }

                return new Tuple<bool, object>(true, value);
            case DataTypeEnum.Integer:
                var successParse = false;
                long intValue = 0;
                while (!successParse)
                {
                    successParse = Int64.TryParse(value, out intValue);
                    if (!successParse)
                    {
                        Console.Write($" Неверный формат! Ожидается целое число");
                        value = Console.ReadLine();
                    }
                }

                return new Tuple<bool, object>(true, intValue);
            case DataTypeEnum.DateTime:
                successParse = false;
                DateTime date = DateTime.Now;
                while (!successParse)
                {
                    successParse = DateTime.TryParse(value, out date);
                    if (!successParse)
                    {
                        Console.Write($" Неверный формат! Ожидается дата в формате \"дд.мм.гггг чч:мм:сс\"");
                        value = Console.ReadLine();
                    }
                }

                return new Tuple<bool, object>(true, date);
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Импортировать в txt.
    /// </summary>
    private void ImportToTxt()
    {
        Console.WriteLine("Импорт");
        var result = _meetingCommands.ImportMeetings();
    //    var cursor = Console.GetCursorPosition();
     //   Console.SetCursorPosition(0, cursor.Top - 1);
      //  Console.Write("\r" + new string(' ', Console.BufferWidth) + "\r");
        if (result.Item1)
            Console.WriteLine("Файл успешно импортирован.");
        else
            Console.WriteLine($"Произошли неполадки: {result.Item2}");
    }
}