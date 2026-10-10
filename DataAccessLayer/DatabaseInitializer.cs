namespace DataAccessLayer
{
    /// <summary>
    /// Подготавливает базу данных к работе: создаёт саму БД и таблицы,
    /// если их ещё нет. Так не приходится создавать таблицы вручную
    /// (и ошибиться в названиях полей или типах данных).
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Создаёт базу данных и таблицы, если они отсутствуют.
        /// Если всё уже создано — ничего не делает.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public static void Initialize(string connectionString)
        {
            using DataContext context = new DataContext(connectionString);
            context.Database.EnsureCreated();
        }
    }
}
