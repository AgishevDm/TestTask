using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyMeetings
{
    // Класс встречи
    public class Meeting
    {
        public DateTime StartTime { get; set; }   // Свойство для хранения времени начала встречи
        public DateTime EndTime { get; set; }     // Свойство для хранения времени окончания встречи
        public string Title { get; set; }         // Свойство для хранения названия встречи
        public TimeSpan Reminder { get; set; }    // Свойство для хранения времени напоминания

        public Meeting(DateTime startTime, DateTime endTime, string title, TimeSpan reminderTime)   // Конструктор класса инициализирующий свойства встречи
        {
            StartTime = startTime;      // Установка времени начала встречи
            EndTime = endTime;          // Установка времени окончания встречи
            Title = title;              // Установка названия встречи
            Reminder = reminderTime;    // Установка времени напоминания
        }

        // Метод для проверки пересечения с другой встречей
        public bool OverlapsWith(Meeting otherMeeting)
        {
            return (StartTime >= otherMeeting.StartTime && StartTime < otherMeeting.EndTime) ||     // Время начала текущей встречи находится в диапазоне другой встречи
                   (EndTime > otherMeeting.StartTime && EndTime <= otherMeeting.EndTime);           // Время окончания текущей встречи находится в диапазоне другой встречи
        }
    }
}
