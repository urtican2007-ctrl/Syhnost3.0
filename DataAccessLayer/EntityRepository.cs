using Microsoft.EntityFrameworkCore;
using Syhnost;

namespace DataAccessLayer
{
    /// <summary>
    /// Репозиторий для работы с базой данных через Entity Framework.
    /// Универсальный: подходит для любой сущности, реализующей <see cref="IDomainObject"/>.
    /// </summary>
    /// <typeparam name="T">Тип сущности предметной области.</typeparam>
    public class EntityRepository<T> : IRepository<T> where T : class, IDomainObject, new()
    {
        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Создаёт репозиторий со строкой подключения по умолчанию.
        /// </summary>
        public EntityRepository() : this(DbSettings.ConnectionString) { }

        /// <summary>
        /// Создаёт репозиторий с указанной строкой подключения
        /// и при необходимости создаёт базу данных и таблицы.
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных.</param>
        public EntityRepository(string connectionString)
        {
            _connectionString = connectionString;
            DatabaseInitializer.Initialize(_connectionString);
        }

        /// <summary>
        /// Создаёт новый контекст. Контекст живёт только на время одной операции
        /// (using), поэтому соединение с БД не «висит» постоянно.
        /// </summary>
        /// <returns>Новый контекст данных.</returns>
        private DataContext CreateContext()
        {
            return new DataContext(_connectionString);
        }

        /// <inheritdoc/>
        public void Add(T obj)
        {
            using DataContext context = CreateContext();
            context.Set<T>().Add(obj);
            context.SaveChanges();
        }

        /// <inheritdoc/>
        public void Delete(T obj)
        {
            using DataContext context = CreateContext();
            T? origin = context.Set<T>().Find(obj.Id);
            if (origin == null)
            {
                return;
            }

            context.Set<T>().Remove(origin);
            context.SaveChanges();
        }

        /// <inheritdoc/>
        public IEnumerable<T> ReadAll()
        {
            using DataContext context = CreateContext();
            return context.Set<T>().AsNoTracking().ToList();
        }

        /// <inheritdoc/>
        public T? ReadById(int id)
        {
            using DataContext context = CreateContext();
            return context.Set<T>().Find(id);
        }

        /// <inheritdoc/>
        public void Update(T obj)
        {
            using DataContext context = CreateContext();
            T? origin = context.Set<T>().Find(obj.Id);
            if (origin == null)
            {
                return;
            }

            context.Entry(origin).CurrentValues.SetValues(obj);
            context.SaveChanges();
        }
    }
}
