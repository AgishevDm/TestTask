using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyMeetings
{
    // Интерфейс для уведомлений
    public interface Notification
    {
        void Notify(string message);                                                                       // Метод для вывода уведомлений в консоль
        void SetReminder(DateTime reminderTime, string message);                                           // Метод для установки напоминания
    }

    // Базовый класс для уведомлений
    public abstract class NotificationService : Notification
    {
        public abstract void Notify(string message);                                                       // Абстрактный метод для вывода уведомлений в консоль
        public abstract void SetReminder(DateTime reminderTime, string message);                           // Абстрактный метод для установки напоминания
    }

    // Класс для уведомлений в консоли
    public class ConsoleNotificationService : NotificationService
    {
        private System.Timers.Timer reminderTimer = new System.Timers.Timer();                             // Инициализируем таймер

        public override void Notify(string message)                                                        // Вывод уведомлений в консоль
        {
            Console.WriteLine(message);
        }

        public override void SetReminder(DateTime reminderTime,  string message)
        {
            reminderTimer = new System.Timers.Timer((reminderTime - DateTime.Now).TotalMilliseconds);      // Создаем таймер

            reminderTimer.Elapsed += (sender, e) =>                                                        // Устанавливаем обработчик события таймера
            {
                reminderTimer.Stop();    // Останавливаем таймер
                Notify(message);         // Выводим напоминание
            };
            reminderTimer.Start();       // Запускаем таймер
        }
    }
}

