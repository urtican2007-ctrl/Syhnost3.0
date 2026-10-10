namespace DataAccessLayer
{
    /// <summary>
    /// Настройки подключения к базе данных.
    /// Строка подключения хранится в ОДНОМ месте и используется
    /// и Entity Framework, и Dapper.
    /// </summary>
    /// <remarks>
    /// Используется SQL Server LocalDB (входит в Visual Studio).
    /// База данных PharmacyDb создаётся автоматически при первом запуске,
    /// путь к файлам прописывать не нужно — проект работает на любом компьютере.
    /// Если нужен именно файл .mdf в проекте (как в шпаргалке), замените строку на:
    /// @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Путь\К\Pharmacy.mdf;Integrated Security=True;TrustServerCertificate=True"
    /// </remarks>
    public static class DbSettings
    {
        /// <summary>
        /// Строка подключения к LocalDB (база PharmacyDb).
        /// </summary>
        public const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=PharmacyDb;Trusted_Connection=True;TrustServerCertificate=True";
    }
}
