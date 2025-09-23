
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace YearCalendar
{
    // Режим работы: пользовательский (false) или режим разработчика (true)
    enum Mode { User, Developer }

    // Хранение комментария к конкретной дате
    class DateComment
    {
        public DateTime Date { get; set; }
        public string Comment { get; set; }

        public DateComment(DateTime date, string comment)
        {
            Date = date.Date;
            Comment = comment;
        }
    }

    // Основной календарь на год
    class YearCalendar
    {
        public int Year { get; private set; }
        // Если null - использовать стандартное правило високосности Gregorian
        public bool? LeapYearOverride { get; set; }

        // Комментарии к датам
        private Dictionary<DateTime, string> dateComments = new Dictionary<DateTime, string>();

        public YearCalendar(int year)
        {
            Year = year;
            LeapYearOverride = null;
        }

        // Методы работы с календарём

        public bool IsLeapYear(int y)
        {
            // Если задано переопределение, используем его
            if (LeapYearOverride.HasValue)
            {
                // true => leap, false => non-leap
                return LeapYearOverride.Value;
            }
            // Стандартное правило Григорианского календаря
            return (y % 4 == 0) && ((y % 100 != 0) || (y % 400 == 0));
        }

        public int DaysInMonth(int month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
            switch (month)
            {
                case 1: return 31;
                case 2: return IsLeapYear(Year) ? 29 : 28;
                case 3: return 31;
                case 4: return 30;
                case 5: return 31;
                case 6: return 30;
                case 7: return 31;
                case 8: return 31;
                case 9: return 30;
                case 10: return 31;
                case 11: return 30;
                case 12: return 31;
                default: throw new ArgumentOutOfRangeException(nameof(month));
            }
        }

        // Можно добавить комментарий к дате
        public void AddComment(DateTime date, string comment)
        {
            if (date.Year != Year) throw new ArgumentException("Дата должна быть в заданном году");
            dateComments[date.Date] = comment?.Trim();
        }

        public void RemoveComment(DateTime date)
        {
            dateComments.Remove(date.Date);
        }

        public string GetComment(DateTime date)
        {
            if (dateComments.TryGetValue(date.Date, out var c)) return c;
            return null;
        }

        public List<DateComment> GetAllComments()
        {
            return dateComments
                .Select(kv => new DateComment(kv.Key, kv.Value))
                .ToList();
        }

        // Выходные дни для месяца
        public List<DateTime> GetWeekendsOfMonth(int month)
        {
            List<DateTime> weekends = new List<DateTime>();
            int days = DaysInMonth(month);
            for (int d = 1; d <= days; d++)
            {
                var dt = new DateTime(Year, month, d);
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                    weekends.Add(dt);
            }
            return weekends;
        }

        // Форматированный календарь месяца как строка (для вывода)
        public string RenderMonth(int month)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));

            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{monthName} {Year}");
            sb.AppendLine("Пн Вт Ср Чт Пт Сб Вс");

            // опорная дата: первый день месяца
            var first = new DateTime(Year, month, 1);
            // День недели для первого дня (Mon=Monday)
            int offset = (int)first.DayOfWeek;
            // В .NET Week starts on Sunday, но мы хотим начать с понедельника
            // Приведём к формату: понедельник = 1, воскресенье = 0
            int startCol = first.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)first.DayOfWeek - 1;

            int days = DaysInMonth(month);
            int day = 1;

            // добавим пробелы для первых дней
            for (int i = 0; i < startCol; i++)
            {
                sb.Append("   ");
            }

            while (day <= days)
            {
                var dt = new DateTime(Year, month, day);
                bool isWeekend = dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday;
                string dayStr = day.ToString().PadLeft(2, ' ');

                // пометки: если есть комментарий
                var comment = GetComment(dt);
                string marker = isWeekend ? "*" : " ";

                // пометка о комментарии: если есть, ставим знак !
                if (!string.IsNullOrEmpty(comment))
                {
                    // пометка к дате с комментариями
                    marker = "!";
                }

                sb.AppendFormat("{0}{1} ", dayStr, marker);

                // следующий день
                day++;

                // переход на новую строку после воскресенья
                int currentColumn = ((day - 1) + startCol) % 7;
                if (currentColumn == 0)
                {
                    sb.AppendLine();
                }
            }

            sb.AppendLine();
            sb.AppendLine("Примечания: * выходной, ! есть комментарий к дате");
            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Календарь на год");
            int year;
            while (true)
            {
                Console.Write("Введите год (например 2025): ");
                var s = Console.ReadLine();
                if (int.TryParse(s, out year) && year > 0) break;
                Console.WriteLine("Неверный год. Попробуйте ещё раз.");
            }

            Mode mode = SelectMode(out bool devMode);

            YearCalendar yc = new YearCalendar(year);
            if (devMode)
            {
                // Пример: попросим пользователя выбрать режим високосности
                while (true)
                {
                    Console.Write("Разработчик: используем ли принудительный високосный год? (y/n): ");
                    var ans = Console.ReadLine().Trim().ToLower();
                    if (ans == "y" || ans == "yes")
                    {
                        yc.LeapYearOverride = true;
                        break;
                    }
                    if (ans == "n" || ans == "no")
                    {
                        yc.LeapYearOverride = false;
                        break;
                    }
                    Console.WriteLine("Пожалуйста, введите y или n.");
                }
            }

            // Основной цикл взаимодействия
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine($"Год: {year}  Високосный по настройке: {IsLeapDisplay(yc)}");
                Console.WriteLine("Доступные команды:");
                Console.WriteLine("  show [month] - показать календарь месяца (1-12)");
                Console.WriteLine("  day [month] [day] - подробности по дате");
                Console.WriteLine("  addc [month] [day] [comment] - добавить комментарий к дате");
                Console.WriteLine("  clearc [month] [day] - удалить комментарий по дате");
                Console.WriteLine("  weekends [month] - показать выходные месяца");
                Console.WriteLine("  listc - список всех комментариев");
                Console.WriteLine("  export - экспорт комментариев в CSV");
                Console.WriteLine("  help - помощь");
                Console.WriteLine("  exit - выход");
                Console.Write("Введите команду: ");

                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var cmd = parts[0].ToLower();

                try
                {
                    switch (cmd)
                    {
                        case "show":
                            {
                                int month = int.Parse(parts[1]);
                                Console.WriteLine(yc.RenderMonth(month));
                                break;
                            }
                        case "day":
                            {
                                int month = int.Parse(parts[1]);
                                int day = int.Parse(parts[2]);
                                ShowDateInfo(yc, month, day);
                                break;
                            }
                        case "addc":
                            {
                                int month = int.Parse(parts[1]);
                                int day = int.Parse(parts[2]);
                                string comment = input.Substring(input.IndexOf(parts[2]) + parts[2].Length).Trim();
                                if (string.IsNullOrWhiteSpace(comment))
                                {
                                    Console.Write("Комментарий: ");
                                    comment = Console.ReadLine();
                                }
                                var dt = new DateTime(year, month, day);
                                yc.AddComment(dt, comment);
                                Console.WriteLine("Комментарий добавлен.");
                                break;
                            }
                        case "clearc":
                            {
                                int month = int.Parse(parts[1]);
                                int day = int.Parse(parts[2]);
                                var dt = new DateTime(year, month, day);
                                yc.RemoveComment(dt);
                                Console.WriteLine("Комментарий удалён (если он был).");
                                break;
                            }
                        case "weekends":
                            {
                                int month = int.Parse(parts[1]);
                                var w = yc.GetWeekendsOfMonth(month);
                                Console.WriteLine($"Выходные в месяце {month}:");
                                foreach (var dt in w)
                                {
                                    Console.WriteLine(dt.ToString("ddd dd MMM"));
                                }
                                break;
                            }
                        case "listc":
                            {
                                var list = yc.GetAllComments();
                                if (list.Count == 0)
                                {
                                    Console.WriteLine("Комментариев пока нет.");
                                }
                                else
                                {
                                    foreach (var c in list)
                                    {
                                        Console.WriteLine($"{c.Date:yyyy-MM-dd}: {c.Comment}");
                                    }
                                }
                                break;
                            }
                        case "export":
                            {
                                string file = $"comments_{year}.csv";
                                ExportToCsv(yc, file);
                                Console.WriteLine($"Экспортировано в {file}");
                                break;
                            }
                        case "help":
                            {
                                ShowHelp();
                                break;
                            }
                        case "exit":
                            {
                                exit = true;
                                break;
                            }
                        default:
                            Console.WriteLine("Неизвестная команда. Введите help для списка команд.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            Console.WriteLine("Выход. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static string IsLeapDisplay(YearCalendar yc)
        {
            // Простой способ: проверить реальный год по override
            bool leap = false;
            if (yc.LeapYearOverride.HasValue)
                leap = yc.LeapYearOverride.Value;
            // если override нет и год високосный по правилу, тоже можно показать
            if (!yc.LeapYearOverride.HasValue)
                leap = (DateTime.IsLeapYear(yc.Year));
            return leap ? "Да" : "Нет";
        }

        static Mode SelectMode(out bool devMode)
        {
            while (true)
            {
                Console.Write("Выбрать режим (user/developer): ");
                var s = Console.ReadLine().Trim().ToLower();
                if (s == "user" || s == "u")
                {
                    devMode = false;
                    return Mode.User;
                }
                if (s == "developer" || s == "d" || s == "dev")
                {
                    devMode = true;
                    return Mode.Developer;
                }
                Console.WriteLine("Неверный ввод. Попробуйте 'user' или 'developer'.");
            }
        }

        static void ShowDateInfo(YearCalendar yc, int month, int day)
        {
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
            int days = yc.DaysInMonth(month);
            if (day < 1 || day > days) throw new ArgumentOutOfRangeException(nameof(day));

            var dt = new DateTime(yc.Year, month, day);
            Console.WriteLine(dt.ToString("dddd, dd MMMM yyyy", CultureInfo.CurrentCulture));
            var weekend = (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday) ? "Выходной" : "Рабочий";
            Console.WriteLine("День недели: " + dt.DayOfWeek);
            Console.WriteLine("Тип дня: " + weekend);
            var comment = yc.GetComment(dt);
            if (!string.IsNullOrWhiteSpace(comment))
            {
                Console.WriteLine("Комментарий: " + comment);
            }
            else
            {
                Console.WriteLine("Комментариев к дате нет.");
            }
        }

        static void ExportToCsv(YearCalendar yc, string path)
        {
            // экспорт комментариев: дата, комментарий
            var list = yc.GetAllComments()
                .OrderBy(c => c.Date)
                .ToList();

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("Date,Comment");
                foreach (var c in list)
                {
                    // экранируем кавычками
                    string date = c.Date.ToString("yyyy-MM-dd");
                    string comment = c.Comment?.Replace("\"", "\"\"") ?? "";
                    writer.WriteLine($"\"{date}\",\"{comment}\"");
                }
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("  show [month] - показать календарь месяца (1-12)");
            Console.WriteLine("  day [month] [day] - подробности по дате");
            Console.WriteLine("  addc [month] [day] [comment] - добавить комментарий к дате");
            Console.WriteLine("  clearc [month] [day] - удалить комментарий по дате");
            Console.WriteLine("  weekends [month] - показать выходные месяца");
            Console.WriteLine("  listc - список всех комментариев");
            Console.WriteLine("  export - экспорт комментариев в CSV");
            Console.WriteLine("  help - помощь");
            Console.WriteLine("  exit - выход");
        }
    }
}
