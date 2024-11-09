using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyMeetings
{
    // Класс для управления встречами
    public class MeetingManager
    {
        public List<Meeting> meetings = new List<Meeting>();
        private Notification notificationService;

        public MeetingManager(Notification notificationService)
        {
            this.notificationService = notificationService; // Инициализация локальной переменной для уведомлений
        }

        // Метод для установки напоминания для конкретной встречи
        private void SetReminderForMeeting(Meeting meeting)
        {
            // Высчитываем время напоминания
            DateTime reminderDateTime = meeting.StartTime.Date + meeting.Reminder;
            // Вызываем метод для установки напоминания
            notificationService.SetReminder(reminderDateTime, $"\nНапоминание: Встреча '{meeting.Title}' начинается в {meeting.StartTime:HH:mm}\n"); 
        }

        //Добавление встречи с напоминанием
        public void AddMeeting(DateTime startTime, DateTime endTime, string title, TimeSpan reminderTime)
        {
            if (meetings.Any(m => m.OverlapsWith(new Meeting(startTime, endTime, title, reminderTime))))   // Проверка на пересечение с существующими встречами
            {
                notificationService.Notify("Ошибка: Встреча пересекается с другой встречей.");
                return;
            }

            meetings.Add(new Meeting(startTime, endTime, title, reminderTime));  // Добавление встречи
            notificationService.Notify($"Встреча '{title}' добавлена.");         // Уведомление о том, что встреча добавлена
            SetReminderForMeeting(meetings.Last());                              // Используем метод для установки напоминания
        }
      
        // Изменение встречи
        public void EditMeeting(int index, DateTime startTime, DateTime endTime, string title, TimeSpan reminderTime)
        {
            // Проверка на пересечение с существующими встречами
            if (meetings.Any(m => m != meetings[index] && m.OverlapsWith(new Meeting(startTime, endTime, title, reminderTime))))
            {
                notificationService.Notify("Ошибка: Встреча пересекается с другой встречей.");
                return;
            }
            // Проверка если индекс в диапазоне коллекции
            if (index >= 0 && index < meetings.Count)
            {
                // Обновляем данные встречи
                meetings[index].StartTime = startTime;
                meetings[index].EndTime = endTime;
                meetings[index].Title = title;
                meetings[index].Reminder = reminderTime;
                // Вызываем метод для вывода уведомления
                notificationService.Notify($"Встреча '{title}' изменена."); 
                // Установка нового напоминания для измененной встречи
                SetReminderForMeeting(meetings[index]); 
                notificationService.Notify($"Встреча '{title}' изменена. Новое время начала: {startTime:HH:mm}"); 
            }
            else
            {  
                notificationService.Notify("Ошибка: Неверный номер встречи."); 
            }
        }

        // Удаление встречи
        public void DeleteMeeting(int index)
        {
            meetings.RemoveAt(index); // Метод удаляет элемент по индексу
            notificationService.Notify("Встреча удалена.");
        }

        // Просмотр расписания
        public void ShowList(DateTime date)
        {
            notificationService.Notify($"Расписание на {date.ToShortDateString()}:");
            var dayMeetings = meetings.Where(m => m.StartTime.Date == date.Date).ToList(); // Получение списка встреч в заданный день
            if (dayMeetings.Count == 0) // Проверка есть ли в коллекции встречи
            {
                notificationService.Notify("В этот день нет встреч.");
            }
            else
            {
                foreach (var meeting in dayMeetings) // Вывод списка запланированных встреч
                {
                    notificationService.Notify($"\t{meeting.StartTime.ToShortTimeString()} - {meeting.EndTime.ToShortTimeString()}: {meeting.Title}");
                }
            }
        }

        // Экспорт расписания в файл
        public void Export(DateTime date, string filename)
        {
            var dayMeetings = meetings.Where(m => m.StartTime.Date == date.Date).ToList(); // Получение списка встреч в заданный день
            if (dayMeetings.Count == 0)
            {
                notificationService.Notify("В этот день нет встреч.");
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(filename)) // Создание StreamWriter для записи в файл с указанным именем
                {
                    writer.WriteLine($"Расписание на {date.ToShortDateString()}:");  // Запись заголовка с датой в файл
                    foreach (var meeting in dayMeetings)  // Перебор встреч и запись их данных в файл
                    {
                        writer.WriteLine($"\t{meeting.StartTime.ToShortTimeString()} - {meeting.EndTime.ToShortTimeString()}: {meeting.Title}");
                    }
                }
                notificationService.Notify($"Расписание экспортировано в файл '{filename}'.");
            }
        }
    }
}
