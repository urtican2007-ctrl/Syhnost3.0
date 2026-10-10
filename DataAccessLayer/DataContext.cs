using Microsoft.EntityFrameworkCore;
using Syhnost;

namespace DataAccessLayer
{
    /// <summary>
    /// Контекст Entity Framework — «единица работы» (Unit of Work).
    /// Связывает сущности предметной области с таблицами базы данных.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Строка подключения, с которой работает контекст.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Таблица «Таблетки» (Pills) в базе данных.
        /// </summary>
        public DbSet<Pill> Pills { get; set; } = null!;

        /// <summary>
        /// Создаёт контекст со строкой подключения по умолчанию
        /// (<see cref="DbSettings.ConnectionString"/>).
        /// </summary>
        public DataContext() : this(DbSettings.ConnectionString) { }

        /// <summary>
        /// Создаёт контекст с указанной строкой подключения.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Настраивает подключение: используется SQL Server (LocalDB).
        /// </summary>
        /// <param name="optionsBuilder">Построитель параметров контекста.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        /// <summary>
        /// Описывает отображение сущностей на таблицы.
        /// Имя таблицы совпадает с соглашением, которое использует DapperRepository
        /// (имя класса + «s»), поэтому оба репозитория работают с одной таблицей.
        /// </summary>
        /// <param name="modelBuilder">Построитель модели данных.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pill>(entity =>
            {
                entity.ToTable("Pills");
                entity.HasKey(p => p.Id);
            });
        }
    }
}
