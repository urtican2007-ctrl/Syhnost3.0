using System.Data;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;
using Syhnost;

namespace DataAccessLayer
{
    /// <summary>
    /// Репозиторий для работы с базой данных через Dapper.
    /// Универсальный: SQL-запросы собираются по свойствам сущности,
    /// а Dapper выполняет их и сопоставляет (маппит) результат с объектами C#.
    /// Имя таблицы — имя класса + «s» (для Pill это таблица Pills).
    /// </summary>
    /// <typeparam name="T">Тип сущности предметной области.</typeparam>
    public class DapperRepository<T> : IRepository<T> where T : class, IDomainObject, new()
    {
        /// <summary>
        /// Имя таблицы в базе данных.
        /// </summary>
        private static readonly string TableName = typeof(T).Name + "s";

        /// <summary>
        /// Свойства сущности, которые хранятся в столбцах таблицы (кроме Id).
        /// </summary>
        private static readonly PropertyInfo[] DataProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.Name != nameof(IDomainObject.Id))
            .ToArray();

        /// <summary>
        /// Список столбцов для INSERT: [Name], [Manufacturer], ...
        /// </summary>
        private static readonly string ColumnList =
            string.Join(", ", DataProperties.Select(p => $"[{p.Name}]"));

        /// <summary>
        /// Список параметров для INSERT: @Name, @Manufacturer, ...
        /// </summary>
        private static readonly string ParameterList =
            string.Join(", ", DataProperties.Select(p => "@" + p.Name));

        /// <summary>
        /// Список присваиваний для UPDATE: [Name] = @Name, ...
        /// </summary>
        private static readonly string AssignmentList =
            string.Join(", ", DataProperties.Select(p => $"[{p.Name}] = @{p.Name}"));

        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Создаёт репозиторий со строкой подключения по умолчанию.
        /// </summary>
        public DapperRepository() : this(DbSettings.ConnectionString) { }

        /// <summary>
        /// Создаёт репозиторий с указанной строкой подключения
        /// и при необходимости создаёт базу данных и таблицы.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
            DatabaseInitializer.Initialize(_connectionString);
        }

        /// <summary>
        /// Создаёт подключение к базе данных. Подключение закрывается
        /// автоматически благодаря using в вызывающих методах.
        /// </summary>
        /// <returns>Новое подключение (Dapper откроет его сам).</returns>
        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <inheritdoc/>
        public void Add(T obj)
        {
            string script = $"INSERT INTO [{TableName}] ({ColumnList}) " +
                            $"OUTPUT INSERTED.[Id] VALUES ({ParameterList})";

            using IDbConnection db = CreateConnection();
            obj.Id = db.ExecuteScalar<int>(script, obj);
        }

        /// <inheritdoc/>
        public void Delete(T obj)
        {
            string script = $"DELETE FROM [{TableName}] WHERE [Id] = @Id";

            using IDbConnection db = CreateConnection();
            db.Execute(script, new { obj.Id });
        }

        /// <inheritdoc/>
        public IEnumerable<T> ReadAll()
        {
            string script = $"SELECT * FROM [{TableName}]";

            using IDbConnection db = CreateConnection();
            return db.Query<T>(script).ToList();
        }

        /// <inheritdoc/>
        public T? ReadById(int id)
        {
            string script = $"SELECT * FROM [{TableName}] WHERE [Id] = @Id";

            using IDbConnection db = CreateConnection();
            return db.QueryFirstOrDefault<T>(script, new { Id = id });
        }

        /// <inheritdoc/>
        public void Update(T obj)
        {
            string script = $"UPDATE [{TableName}] SET {AssignmentList} WHERE [Id] = @Id";

            using IDbConnection db = CreateConnection();
            db.Execute(script, obj);
        }
    }
}
