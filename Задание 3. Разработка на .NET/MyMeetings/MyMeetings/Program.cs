using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;
//Класс для ввода информации в консоль + проверка ввода 
namespace MyMeetings
{
    public class Program
    {
        private static  MeetingManager meetingManager = new MeetingManager(new ConsoleNotificationService());
       
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в менеджер встреч!");
            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Добавить встречу");
                Console.WriteLine("2. Изменить встречу");
                Console.WriteLine("3. Удалить встречу");
                Console.WriteLine("4. Посмотреть расписание");
                Console.WriteLine("5. Экспортировать расписание");
                Console.WriteLine("6. Выход");
                Console.Write("Введите номер действия: ");

                if (int.TryParse(Console.ReadLine(), out int x))                                                                  // Проверяем введено ли (int)-значение 
                {
                    switch (x) 
                    {
                        case 1:
                            AddMeeting(); 
                            break;
                        case 2:
                            EditMeeting();
                            break;
                        case 3:
                            DeleteMeeting();
                            break;
                        case 4:
                            ShowList();
                            break;
                        case 5:
                            Export();
                            break;
                        case 6:
                            Console.WriteLine("До свидания!");
                            return;
                        default:
                            Console.WriteLine("Неверный номер действия.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Неверный ввод. Введите числовое значение.");
                }
            }
        }

        // Добавление встречи
        static void AddMeeting()
        {
            DateTime startTime;
            DateTime endTime;
            string title;
            TimeSpan reminderTime;

            while (true)                                                                                                          // Пока не ввели корректное время начала встречи
            {   
                Console.Write("Введите время начала встречи (ДД.ММ.ГГГГ ЧЧ:ММ): ");                                              
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null, DateTimeStyles.None, out startTime))     // Условие для проверки введенного формата даты 
                {
                    if (startTime > DateTime.Now)                                                                                 // Условие проверки того, что вводимая дата в будущем 
                    { 
                        break;                                                                                                    // Выход из цикла, если дата начала введена верно и в будущем
                    } 
                    else { Console.WriteLine("Время начала встречи должно быть в будущем. Попробуйте снова."); }
                }
                else { Console.WriteLine("Неверный формат времени начала встречи. Попробуйте снова."); }
            }

            while (true)                                                                                                          // Пока не ввели корректное время окончания встречи
            {
                Console.Write("Введите время окончания встречи (ДД.ММ.ГГГГ ЧЧ:ММ): ");                                            
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null, DateTimeStyles.None, out endTime))       // Условие для проверки введенного формата даты
                {
                    if (endTime > DateTime.Now && endTime > startTime)                                                            // Условие проверки того, что время окончания в будущем и после времени начала
                    {
                        break;                                                                                                    // Выход из цикла, если время введено верно и в будущем
                    }
                    else { Console.WriteLine("Время окончания встречи должно быть после времени начала. Попробуйте снова."); }
                }
                else { Console.WriteLine("Неверный формат времени окончания встречи. Попробуйте снова."); }
            }

            while (true)                                                                                                          // Пока не получили не пустое название встречи
            {
                Console.Write("Введите название встречи: ");
                title = Console.ReadLine();                                                                                       // Вводим название встречи

                if (!string.IsNullOrEmpty(title))                                                                                 // Условие проверки title <> null
                {
                    break;                                                                                                        // Выход из цикла, если название введено верно
                }
                else { Console.WriteLine("Название встречи не может быть пустым. Попробуйте снова."); }
            }

            while (true)
            {
                Console.Write("Введите время напоминания (ЧЧ:ММ): ");                                                             // Ввод времени напоминания

                if (TimeSpan.TryParse(Console.ReadLine(), out reminderTime))                                                      // Для проверки верного формата времени напоминания
                {
                    DateTime reminderDateTime = startTime.Date.Add(reminderTime);                                                 // Создаем DateTime из TimeSpan

                    if (reminderDateTime < startTime)                                                                             // Проверка, что время напоминания не позже времени начала встречи
                    {
                        break;                                                                                                    // Выход из цикла, если время введено верно
                    }
                    else { Console.WriteLine("Время напоминания не может быть позже времени начала встречи. Попробуйте снова.");}
                }
                else { Console.WriteLine("Неверный формат времени напоминания. Попробуйте снова."); }
            }
            meetingManager.AddMeeting(startTime, endTime, title, reminderTime);                                                   // Вызов функции для добавления встречи
        }

        // Изменение встречи
        static void EditMeeting()
        {
            Console.WriteLine("Список встреч:");                                                                                  // Список встреч в консоли

            if (meetingManager.meetings.Count == 0)                                                                               // Условие для проверки наличия встреч
            {
                Console.WriteLine("Список встреч пуст.");                                                                         // Если список пуст в консоли выходит сообщение и производится 
                return;                                                                                                           // Выходим из метода
            }
            for (int i = 0; i < meetingManager.meetings.Count; i++)                                                               // Цикл для вывода информации о каждой встрече коллекции
            {
                Console.WriteLine($"{i + 1}. " +                                                                                  // В консоль выводится номер встречи
                    $"{meetingManager.meetings[i].StartTime.ToShortDateString()}" +                                               // Дата встречи
                    $" {meetingManager.meetings[i].StartTime.ToShortTimeString()} " +                                             // Время встречи
                    $"- {meetingManager.meetings[i].Title}");                                                                     // Название встречи
            }

            int index = -1;                                                                                                       // Инициализируем -1, чтобы войти в цикл

            while (index < 0 || index >= meetingManager.meetings.Count)                                                           // Находимся в цикле пока пользователь не введет корректный номер встречи для изменения
            {
                Console.Write("Введите номер встречи для изменения: ");
                if (int.TryParse(Console.ReadLine(), out index))                                                                  // Проверка ввел ли пользователь (int)
                {
                    index--;                                                                                                      // Декрементируем индекс массива (т.к индекс начинается с 0)
                    if (index < 0 || index >= meetingManager.meetings.Count)
                    {
                        Console.WriteLine("Неверный номер встречи. Попробуйте снова.");                                           // Для вывода ошибоки при некорректном вводе 
                    }
                }
                else { Console.WriteLine("Неверный ввод. Введите числовое значение."); }
            }
           
            DateTime startTime = new DateTime();
            while (true)
            {
                Console.Write("Измените время начала встречи (ДД.ММ.ГГГГ ЧЧ:ММ): ");                                              // Ввод времени начала встречи
                 
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null, DateTimeStyles.None, out startTime))     // Условие для проверкм введенного формата
                {
                    if (startTime > DateTime.Now)                                                                                 // Условие для проверки того, что вводимая дата в будущем 
                    {
                        break;                                                                                                    // Выход из цикла, если дата начала введена верно и в будущем
                    }
                    else { Console.WriteLine("Время начала встречи должно быть в будущем. Попробуйте снова."); }                  // Для вывода ошибок при некорректном вводе 
                }
                else { Console.WriteLine("Неверный формат времени начала встречи. Попробуйте снова."); }
            }

            DateTime endTime = new DateTime();
            while (true)
            {
                Console.Write("Измените время окончания встречи (ДД.ММ.ГГГГ ЧЧ:ММ): ");                                           // Ввод времени окончания встречи

                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy HH:mm", null, DateTimeStyles.None, out endTime))       // Условие для проверки введенного формата дат
                {
                    if (endTime > DateTime.Now && endTime > startTime)                                                            // Условие для проверка того, что время окончания в будущем и после времени начала
                    {
                        break;                                                                                                    // Выход из цикла, если время введено верно и в будущем
                    }
                    else { Console.WriteLine("Время окончания встречи должно быть после времени начала. Попробуйте снова."); }    // Для вывода ошибок при некорректном вводе 
                }
                else { Console.WriteLine("Неверный формат времени окончания встречи. Попробуйте снова."); }
            }

            string title = string.Empty;
            while (string.IsNullOrEmpty(title))
            {
                Console.Write("Измените название встречи: ");                                                                      // Ввод названия встречи
                title = Console.ReadLine();
                if (string.IsNullOrEmpty(title))                                                                                   // Условие проверки title<>null
                { Console.WriteLine("Название встречи не может быть пустым. Попробуйте снова."); }
            }

            TimeSpan reminderTime = new TimeSpan();
            while (true)
            {
                Console.Write("Измените время напоминания (ЧЧ:ММ): ");                                                             // Ввод времени напоминания

                if (TimeSpan.TryParse(Console.ReadLine(), out reminderTime))                                                       // Условие для проверки формата вводимого времени
                {
                    DateTime reminderDateTime = startTime.Date.Add(reminderTime);                                                  // Создаем DateTime из TimeSpan

                    if (reminderDateTime < startTime)                                                                              // Проверка, что время напоминания не позже времени начала встречи
                    {
                        break;                                                                                                     // Выход из цикла, если время введено верно 
                    }
                    else { Console.WriteLine("Время напоминания не может быть позже времени начала встречи. Попробуйте снова.");}  // Для вывода ошибок при некорректном вводе 
                }
                else { Console.WriteLine("Неверный формат времени напоминания. Попробуйте снова.");}
            }
            meetingManager.EditMeeting(index, startTime, endTime, title, reminderTime);                                            // Вызов функции для изменение встречи
        }

        // Удаление встречи
        static void DeleteMeeting()
        {
            Console.WriteLine("Список встреч:");                                                                                
            if (meetingManager.meetings.Count == 0)                                                                                // Условие для проверки наличия встреч
            {
                Console.WriteLine("Список встреч пуст.");                                                                          // Если список пуст в консоли выходит сообщение и производится 
                return;                                                                                                            // Выходим из метода
            } 

            for (int i = 0; i < meetingManager.meetings.Count; i++)                                                                // Цикл для вывода информации о каждой встрече коллекции
            {
                Console.WriteLine($"{i + 1}. " +                                                                                   // В консоль выводится номер встречи
                    $"{meetingManager.meetings[i].StartTime.ToShortDateString()} " +                                               // Дата встречи
                    $"{meetingManager.meetings[i].StartTime.ToShortTimeString()} " +                                               // Время встречи
                    $"- {meetingManager.meetings[i].Title}");                                                                      // Название встречи
            }

            int index = -1;                                                                                                        // Инициализируем -1, чтобы войти в цикл
            while (index < 0 || index >= meetingManager.meetings.Count)
            {
                Console.Write("Введите номер встречи для удаления: ");                                                             // Ввод номера встречи

                if (int.TryParse(Console.ReadLine(), out index))                                                                   // Условие для проверки на ввод (int)-значения
                {
                    index--;
                    if (index < 0 || index >= meetingManager.meetings.Count)                                                       // Условие для проверки ввели ли значение относящееся к диапазону списка встреч
                    {
                        Console.WriteLine("Неверный номер встречи. Попробуйте снова.");
                    }
                }
                else { Console.WriteLine("Неверный ввод. Введите числовое значение."); }                                           // Для вывода ошибки при некорректном вводе 
            }
            meetingManager.DeleteMeeting(index);                                                                                   // Вызываем метод для удаление встречи
        }

        // Просмотр расписания
        static void ShowList()
        {
            DateTime date;
            Console.Write("Введите дату (ДД.ММ.ГГГГ): ");                                                                          
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, DateTimeStyles.None, out date))                 // Для обработки неправильно введенной даты 
            {
                Console.WriteLine("Неверный формат даты. Попробуйте снова.");
            }
            meetingManager.ShowList(date);                                                                                         // Вызываем метод для вывода расписания
        }

        // Экспорт расписания
        static void Export()
        {
            DateTime date;
            while (true)                                                                                                           // Пока не ввели корректно дату
            {
                Console.Write("Введите дату (ДД.ММ.ГГГГ): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, DateTimeStyles.None, out date))
                {
                    break;                                                                                                         // Выход из цикла, если дата введена верно
                }
                else
                {
                    Console.WriteLine("Неверный формат даты. Попробуйте снова.");
                }
            }
            string filename = string.Empty;
            while (string.IsNullOrEmpty(filename))                                                                                 // Пока "имя файла" пустое запрашиваем, чтобы пользователь ввел данные
            {
                Console.Write("Введите имя файла: ");
                filename = Console.ReadLine();
                if (string.IsNullOrEmpty(filename))
                {
                    Console.WriteLine("Имя файла не может быть пустым. Попробуйте снова.");
                }
            }
            meetingManager.Export(date, filename);                                                                                 // Вызываем метод для экспорта расписания
        }
    }
}

