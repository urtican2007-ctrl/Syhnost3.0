using Syhnost;

namespace DataAccessLayer
{
    /// <summary>
    /// Паттерн «Репозиторий»: общий контракт доступа к данным.
    /// Описывает стандартные CRUD-операции и скрывает от бизнес-логики то,
    /// каким именно способом (Entity Framework, Dapper) данные хранятся.
    /// </summary>
    /// <typeparam name="T">Тип сущности предметной области.</typeparam>
    public interface IRepository<T> where T : class, IDomainObject, new()
    {
        /// <summary>
        /// Добавляет новый объект в хранилище.
        /// После добавления свойство <see cref="IDomainObject.Id"/> содержит
        /// идентификатор, присвоенный базой данных.
        /// </summary>
        /// <param name="obj">Добавляемый объект.</param>
        void Add(T obj);

        /// <summary>
        /// Удаляет объект из хранилища (поиск по <see cref="IDomainObject.Id"/>).
        /// </summary>
        /// <param name="obj">Удаляемый объект.</param>
        void Delete(T obj);

        /// <summary>
        /// Возвращает все объекты из хранилища.
        /// </summary>
        /// <returns>Коллекция всех объектов.</returns>
        IEnumerable<T> ReadAll();

        /// <summary>
        /// Возвращает объект по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор объекта.</param>
        /// <returns>Найденный объект или null, если такого объекта нет.</returns>
        T? ReadById(int id);

        /// <summary>
        /// Обновляет данные существующего объекта (поиск по <see cref="IDomainObject.Id"/>).
        /// </summary>
        /// <param name="obj">Объект с новыми значениями полей.</param>
        void Update(T obj);
    }
}
