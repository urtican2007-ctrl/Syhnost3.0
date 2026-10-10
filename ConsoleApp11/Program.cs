using System.Globalization;
using System.Text;
using BusinessLogic;
using Syhnost;

namespace ConsoleApp11
{
    /// <summary>
    /// Точка входа консольного приложения «Аптека: управление таблетками».
    /// Класс реализует консольный пользовательский интерфейс и делегирует
    /// все операции бизнес-логики объекту <see cref="Logic"/>.
    /// </summary>
    /// <remarks>
    /// Консольное представление является одним из двух вариантов интерфейса
    /// (второй — Windows Forms) и использует общую бизнес-логику.
    /// </remarks>
    internal class Program
    {
        /// <summary>
        /// Главная точка входа в консольное приложение.
        /// Отображает главное меню, обрабатывает выбор пользователя
        /// и вызывает соответствующие методы бизнес-логики.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Logic logic;
            try
            {
                logic = new Logic();
                logic.SeedDemoData();
            }
            catch (Exception ex)
            {
                PrintFatalError(ex);
                return;
            }

            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                DrawHeader("АПТЕКА: УПРАВЛЕНИЕ ТАБЛЕТКАМИ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  Главное меню:");
                Console.ResetColor();
                Console.WriteLine("  ┌──────────────────────────────────────┐");
                Console.WriteLine("  │ 1. Добавить таблетку                 │");
                Console.WriteLine("  │ 2. Показать все таблетки             │");
                Console.WriteLine("  │ 3. Удалить таблетку                  │");
                Console.WriteLine("  │ 4. Изменить таблетку                 │");
                Console.WriteLine("  │ 5. Показать по производителю         │");
                Console.WriteLine("  │ 6. Показать только рецептурные       │");
                Console.WriteLine("  │ 7. Сортировка по цене (дорогие)      │");
                Console.WriteLine("  ├──────────────────────────────────────┤");
                Console.WriteLine("  │ 0. Выход                             │");
                Console.WriteLine("  └──────────────────────────────────────┘");

                Console.Write("\n  Выберите пункт: ");
                string? choice = Console.ReadLine();

                try
                {
                    switch (choice?.Trim())
                    {
                        case "1": AddPillUI(logic); break;
                        case "2": ShowAllPills(logic); break;
                        case "3": DeletePillUI(logic); break;
                        case "4": UpdatePillUI(logic); break;
                        case "5": SearchByManufacturerUI(logic); break;
                        case "6": ShowPrescriptionPills(logic); break;
                        case "7": SortByPriceDescUI(logic); break;
                        case "0": exit = true; break;
                        default:
                            PrintError("Неверный ввод. Выберите пункт из меню.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintError("Ошибка при работе с базой данных: " + ex.GetBaseException().Message);
                }
            }
        }

        #region Оформление и сообщения

        /// <summary>
        /// Выводит в консоль заголовок приложения в рамке.
        /// </summary>
        /// <param name="title">Текст заголовка, который нужно отобразить.</param>
        static void DrawHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║{title.PadRight(58)}║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        /// <summary>
        /// Ожидает нажатия любой клавиши (пауза перед возвратом в меню).
        /// </summary>
        static void WaitForKey()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  Нажмите любую клавишу для продолжения...");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Выводит сообщение об успешном выполнении операции и ждёт нажатия клавиши.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ✔ {message}");
            Console.ResetColor();
            WaitForKey();
        }

        /// <summary>
        /// Выводит сообщение об ошибке и ждёт нажатия клавиши.
        /// </summary>
        /// <param name="message">Текст сообщения об ошибке.</param>
        static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  ✖ {message}");
            Console.ResetColor();
            WaitForKey();
        }

        /// <summary>
        /// Выводит информационное сообщение и ждёт нажатия клавиши.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  ! {message}");
            Console.ResetColor();
            WaitForKey();
        }

        /// <summary>
        /// Выводит короткую ошибку ввода (без паузы) — после неё поле вводится заново.
        /// </summary>
        /// <param name="message">Текст сообщения об ошибке.</param>
        static void PrintInputError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✖ {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Сообщает о критической ошибке запуска (например, не удалось подключиться к БД).
        /// </summary>
        /// <param name="ex">Возникшее исключение.</param>
        static void PrintFatalError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ✖ Не удалось подключиться к базе данных.");
            Console.ResetColor();
            Console.WriteLine("  Проверьте, что установлен SQL Server LocalDB (компонент Visual Studio).");
            Console.WriteLine($"  Причина: {ex.GetBaseException().Message}");
            WaitForKey();
        }

        #endregion

        #region Ввод данных

        /// <summary>
        /// Преобразует строку в неотрицательное вещественное число.
        /// Принимает и точку, и запятую в качестве разделителя.
        /// </summary>
        /// <param name="input">Введённая строка.</param>
        /// <param name="value">Результат преобразования.</param>
        /// <returns>true, если строка — корректное число не меньше нуля.</returns>
        static bool TryParsePrice(string? input, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string normalized = input.Trim().Replace(',', '.');
            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                   && double.IsFinite(value)
                   && value >= 0;
        }

        /// <summary>
        /// Преобразует ответ «да/нет» в логическое значение.
        /// </summary>
        /// <param name="input">Введённая строка.</param>
        /// <returns>true — «да», false — «нет», null — ответ не распознан.</returns>
        static bool? ParseYesNo(string? input)
        {
            string value = (input ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "да" or "д" or "y" or "yes" => true,
                "нет" or "н" or "n" or "no" => false,
                _ => null
            };
        }

        /// <summary>
        /// Запрашивает у пользователя целое число и повторяет ввод при ошибке.
        /// </summary>
        /// <param name="prompt">Текст приглашения к вводу.</param>
        /// <param name="minValue">Минимально допустимое значение.</param>
        /// <returns>Введённое целое число.</returns>
        static int ReadInt(string prompt, int minValue = int.MinValue)
        {
            while (true)
            {
                Console.Write($"  {prompt}");
                if (int.TryParse(Console.ReadLine()?.Trim(), out int result) && result >= minValue)
                {
                    return result;
                }

                PrintInputError(minValue == int.MinValue
                    ? "Ошибка: нужно ввести целое число!"
                    : $"Ошибка: нужно ввести целое число не меньше {minValue}!");
            }
        }

        /// <summary>
        /// Запрашивает у пользователя неотрицательное число (например, цену)
        /// и повторяет ввод при ошибке.
        /// </summary>
        /// <param name="prompt">Текст приглашения к вводу.</param>
        /// <returns>Введённое число.</returns>
        static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write($"  {prompt}");
                if (TryParsePrice(Console.ReadLine(), out double result)) return result;
                PrintInputError("Ошибка: нужно ввести неотрицательное число (например, 150.50)!");
            }
        }

        /// <summary>
        /// Запрашивает у пользователя непустую строку.
        /// </summary>
        /// <param name="prompt">Текст приглашения к вводу.</param>
        /// <returns>Введённая непустая строка.</returns>
        static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write($"  {prompt}");
                string? result = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(result)) return result.Trim();
                PrintInputError("Ошибка: поле не может быть пустым!");
            }
        }

        /// <summary>
        /// Запрашивает ответ «да/нет» и повторяет ввод при ошибке.
        /// </summary>
        /// <param name="prompt">Текст приглашения к вводу.</param>
        /// <returns>true — «да», false — «нет».</returns>
        static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write($"  {prompt}");
                bool? answer = ParseYesNo(Console.ReadLine());
                if (answer.HasValue) return answer.Value;
                PrintInputError("Ошибка: введите «да» или «нет»!");
            }
        }

        /// <summary>
        /// Запрашивает строку при изменении записи: пустой ввод оставляет текущее значение.
        /// </summary>
        /// <param name="prompt">Название поля.</param>
        /// <param name="current">Текущее значение поля.</param>
        /// <returns>Новое или прежнее значение.</returns>
        static string ReadOptionalString(string prompt, string current)
        {
            Console.Write($"  {prompt} [{current}]: ");
            string? input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? current : input.Trim();
        }

        /// <summary>
        /// Запрашивает цену при изменении записи: пустой ввод оставляет текущее значение.
        /// </summary>
        /// <param name="prompt">Название поля.</param>
        /// <param name="current">Текущее значение поля.</param>
        /// <returns>Новое или прежнее значение.</returns>
        static double ReadOptionalDouble(string prompt, double current)
        {
            while (true)
            {
                Console.Write($"  {prompt} [{current.ToString("F2", CultureInfo.InvariantCulture)}]: ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return current;
                if (TryParsePrice(input, out double result)) return result;
                PrintInputError("Ошибка: нужно ввести неотрицательное число (например, 150.50)!");
            }
        }

        /// <summary>
        /// Запрашивает целое число при изменении записи: пустой ввод оставляет текущее значение.
        /// </summary>
        /// <param name="prompt">Название поля.</param>
        /// <param name="current">Текущее значение поля.</param>
        /// <returns>Новое или прежнее значение.</returns>
        static int ReadOptionalInt(string prompt, int current)
        {
            while (true)
            {
                Console.Write($"  {prompt} [{current}]: ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return current;
                if (int.TryParse(input.Trim(), out int result) && result >= 0) return result;
                PrintInputError("Ошибка: нужно ввести целое число не меньше 0!");
            }
        }

        /// <summary>
        /// Запрашивает ответ «да/нет» при изменении записи: пустой ввод оставляет текущее значение.
        /// </summary>
        /// <param name="prompt">Название поля.</param>
        /// <param name="current">Текущее значение поля.</param>
        /// <returns>Новое или прежнее значение.</returns>
        static bool ReadOptionalYesNo(string prompt, bool current)
        {
            while (true)
            {
                Console.Write($"  {prompt} [{(current ? "да" : "нет")}]: ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return current;
                bool? answer = ParseYesNo(input);
                if (answer.HasValue) return answer.Value;
                PrintInputError("Ошибка: введите «да» или «нет»!");
            }
        }

        #endregion

        #region Экраны меню

        /// <summary>
        /// Интерфейс добавления новой таблетки.
        /// Запрашивает данные у пользователя и передаёт их в бизнес-логику.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики для работы с таблетками.</param>
        static void AddPillUI(Logic logic)
        {
            Console.Clear();
            DrawHeader("ДОБАВЛЕНИЕ ТАБЛЕТКИ");

            string name = ReadString("Название: ");
            string manufacturer = ReadString("Производитель: ");
            double price = ReadDouble("Цена: ");
            int quantity = ReadInt("Количество: ", 0);
            bool isPrescription = ReadYesNo("По рецепту? (да/нет): ");

            logic.AddPill(new Pill(name, manufacturer, price, quantity, isPrescription));

            PrintSuccess("Таблетка успешно добавлена!");
        }

        /// <summary>
        /// Отображает список всех таблеток в виде таблицы.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики для получения списка таблеток.</param>
        static void ShowAllPills(Logic logic)
        {
            Console.Clear();
            DrawHeader("СПИСОК ВСЕХ ТАБЛЕТОК");
            PrintPillsTable(logic.GetAllPills());
            WaitForKey();
        }

        /// <summary>
        /// Интерфейс удаления таблетки по её идентификатору.
        /// Перед вводом ID показывает таблицу со всеми таблетками.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики для удаления таблетки.</param>
        static void DeletePillUI(Logic logic)
        {
            Console.Clear();
            DrawHeader("УДАЛЕНИЕ ТАБЛЕТКИ");

            List<Pill> pills = logic.GetAllPills();
            PrintPillsTable(pills);
            if (pills.Count == 0)
            {
                WaitForKey();
                return;
            }

            Console.WriteLine();
            int id = ReadInt("Введите ID таблетки для удаления: ");

            Pill? pillToDelete = logic.GetPillById(id);
            if (pillToDelete == null)
            {
                PrintError("Таблетка с таким ID не найдена.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  Вы собираетесь удалить: {pillToDelete.Name} (ID: {pillToDelete.Id})");
            Console.ResetColor();

            if (!ReadYesNo("Подтвердить удаление? (да/нет): "))
            {
                PrintInfo("Удаление отменено.");
                return;
            }

            if (logic.DeletePill(id))
                PrintSuccess("Таблетка удалена!");
            else
                PrintError("Не удалось удалить таблетку.");
        }

        /// <summary>
        /// Интерфейс изменения существующей таблетки.
        /// В этом же окне выводится таблица со всеми таблетками и информацией о них,
        /// чтобы пользователь видел, какую запись выбрать.
        /// Пустой ввод в поле сохраняет текущее значение.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики для обновления данных.</param>
        static void UpdatePillUI(Logic logic)
        {
            Console.Clear();
            DrawHeader("ИЗМЕНЕНИЕ ТАБЛЕТКИ");

            // Таблица со всеми таблетками — прямо в окне изменения
            List<Pill> pills = logic.GetAllPills();
            PrintPillsTable(pills);
            if (pills.Count == 0)
            {
                WaitForKey();
                return;
            }

            Console.WriteLine();
            int id = ReadInt("Введите ID таблетки для изменения: ");

            Pill? existing = logic.GetPillById(id);
            if (existing == null)
            {
                PrintError("Таблетка с таким ID не найдена.");
                return;
            }

            Console.WriteLine($"\n  Редактирование: {existing.Name} ({existing.Manufacturer})");
            Console.WriteLine("  (Оставьте поле пустым, чтобы не менять значение)\n");

            string name = ReadOptionalString("Новое название", existing.Name);
            string manufacturer = ReadOptionalString("Новый производитель", existing.Manufacturer);
            double price = ReadOptionalDouble("Новая цена", existing.Price);
            int quantity = ReadOptionalInt("Новое количество", existing.Quantity);
            bool isPrescription = ReadOptionalYesNo("По рецепту? (да/нет)", existing.IsPrescription);

            Pill updatedPill = new Pill(name, manufacturer, price, quantity, isPrescription)
            {
                Id = id
            };

            if (logic.UpdatePill(updatedPill))
            {
                Console.WriteLine();
                PrintPillsTable(logic.GetAllPills());
                PrintSuccess("Изменено успешно!");
            }
            else
            {
                PrintError("Ошибка при изменении.");
            }
        }

        /// <summary>
        /// Интерфейс поиска таблеток по производителю.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики для поиска.</param>
        static void SearchByManufacturerUI(Logic logic)
        {
            Console.Clear();
            DrawHeader("ПОИСК ПО ПРОИЗВОДИТЕЛЮ");

            string manufacturer = ReadString("Введите производителя: ");
            List<Pill> result = logic.GetPillsByManufacturer(manufacturer);

            Console.WriteLine();
            PrintPillsTable(result);
            WaitForKey();
        }

        /// <summary>
        /// Отображает только рецептурные препараты.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики для получения списка рецептурных таблеток.</param>
        static void ShowPrescriptionPills(Logic logic)
        {
            Console.Clear();
            DrawHeader("РЕЦЕПТУРНЫЕ ПРЕПАРАТЫ");
            PrintPillsTable(logic.GetPrescriptionPills());
            WaitForKey();
        }

        /// <summary>
        /// Отображает таблетки, отсортированные по цене от дорогих к дешёвым.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики для получения отсортированного списка.</param>
        static void SortByPriceDescUI(Logic logic)
        {
            Console.Clear();
            DrawHeader("СОРТИРОВКА ПО ЦЕНЕ (ОТ ДОРОГИХ К ДЕШЕВЫМ)");
            PrintPillsTable(logic.GetPillsSortedByPriceDesc());
            WaitForKey();
        }

        #endregion

        #region Таблица

        /// <summary>
        /// Приводит текст к фиксированной ширине столбца:
        /// короткий — дополняет пробелами, длинный — обрезает с многоточием.
        /// </summary>
        /// <param name="text">Исходный текст.</param>
        /// <param name="width">Ширина столбца.</param>
        /// <returns>Строка ровно заданной ширины.</returns>
        static string Fit(string text, int width)
        {
            if (text.Length > width)
            {
                return text.Substring(0, width - 1) + "…";
            }

            return text.PadRight(width);
        }

        /// <summary>
        /// Выводит список таблеток в консоль в виде таблицы.
        /// </summary>
        /// <param name="pills">Список таблеток для отображения.</param>
        static void PrintPillsTable(List<Pill> pills)
        {
            if (pills.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Список пуст.");
                Console.ResetColor();
                return;
            }

            int wId = 5, wName = 25, wMan = 20, wPrice = 10, wQty = 8, wRx = 8;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ┌" + new string('─', wId) + "┬" + new string('─', wName) + "┬" + new string('─', wMan) + "┬" + new string('─', wPrice) + "┬" + new string('─', wQty) + "┬" + new string('─', wRx) + "┐");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  │" + Fit("ID", wId) + "│" + Fit("Название", wName) + "│" + Fit("Производитель", wMan) + "│" + Fit("Цена", wPrice) + "│" + Fit("Кол-во", wQty) + "│" + Fit("Рецепт", wRx) + "│");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ├" + new string('─', wId) + "┼" + new string('─', wName) + "┼" + new string('─', wMan) + "┼" + new string('─', wPrice) + "┼" + new string('─', wQty) + "┼" + new string('─', wRx) + "┤");
            Console.ResetColor();

            foreach (Pill pill in pills)
            {
                string rx = pill.IsPrescription ? "Да" : "Нет";
                Console.WriteLine("  │" + Fit(pill.Id.ToString(), wId) + "│" +
                                  Fit(pill.Name, wName) + "│" +
                                  Fit(pill.Manufacturer, wMan) + "│" +
                                  Fit(pill.Price.ToString("F2"), wPrice) + "│" +
                                  Fit(pill.Quantity.ToString(), wQty) + "│" +
                                  Fit(rx, wRx) + "│");
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  └" + new string('─', wId) + "┴" + new string('─', wName) + "┴" + new string('─', wMan) + "┴" + new string('─', wPrice) + "┴" + new string('─', wQty) + "┴" + new string('─', wRx) + "┘");
            Console.WriteLine($"  Всего записей: {pills.Count}");
            Console.ResetColor();
        }

        #endregion
    }
}
