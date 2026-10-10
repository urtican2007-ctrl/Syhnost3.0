using DataAccessLayer;
using Syhnost;

namespace BusinessLogic
{
    /// <summary>
    /// Класс бизнес-логики для работы с аптечными таблетками.
    /// Содержит операции создания, чтения, изменения и удаления записей,
    /// а также дополнительные функции фильтрации и сортировки.
    /// </summary>
    /// <remarks>
    /// Этот класс не знает, где и как хранятся данные: он работает только
    /// с интерфейсом <see cref="IRepository{T}"/>. Благодаря этому способ хранения
    /// (Entity Framework или Dapper) меняется одной строкой, а представления
    /// (консоль и WinForms) вообще не затрагиваются.
    /// </remarks>
    public class Logic
    {
        /// <summary>
        /// Репозиторий, через который выполняется вся работа с данными.
        /// </summary>
        private readonly IRepository<Pill> _repository;

        /// <summary>
        /// Создаёт бизнес-логику с репозиторием по умолчанию (Entity Framework).
        /// Чтобы перейти на Dapper, достаточно заменить строку создания репозитория.
        /// </summary>
        public Logic()
        {
            _repository = new EntityRepository<Pill>();
            // _repository = new DapperRepository<Pill>();
        }

        /// <summary>
        /// Создаёт бизнес-логику с произвольным репозиторием.
        /// </summary>
        /// <param name="repository">Репозиторий для хранения таблеток.</param>
        public Logic(IRepository<Pill> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Добавляет новую таблетку в хранилище.
        /// Уникальный идентификатор присваивает база данных.
        /// </summary>
        /// <param name="pill">Экземпляр таблетки для добавления.</param>
        public void AddPill(Pill pill)
        {
            _repository.Add(pill);
        }

        /// <summary>
        /// Удаляет таблетку по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой таблетки.</param>
        /// <returns>true, если запись найдена и удалена; иначе false.</returns>
        public bool DeletePill(int id)
        {
            Pill? pillToRemove = _repository.ReadById(id);
            if (pillToRemove == null)
            {
                return false;
            }

            _repository.Delete(pillToRemove);
            return true;
        }

        /// <summary>
        /// Возвращает список всех таблеток.
        /// </summary>
        /// <returns>Новый список, содержащий все таблетки.</returns>
        public List<Pill> GetAllPills()
        {
            return _repository.ReadAll().ToList();
        }

        /// <summary>
        /// Возвращает таблетку по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор таблетки.</param>
        /// <returns>Найденная таблетка или null, если такой нет.</returns>
        public Pill? GetPillById(int id)
        {
            return _repository.ReadById(id);
        }

        /// <summary>
        /// Обновляет данные существующей таблетки.
        /// Поиск выполняется по идентификатору.
        /// </summary>
        /// <param name="updatedPill">Таблетка с новыми данными.</param>
        /// <returns>true, если запись найдена и обновлена; иначе false.</returns>
        public bool UpdatePill(Pill updatedPill)
        {
            if (_repository.ReadById(updatedPill.Id) == null)
            {
                return false;
            }

            _repository.Update(updatedPill);
            return true;
        }

        /// <summary>
        /// Возвращает список таблеток указанного производителя.
        /// Сравнение выполняется без учёта регистра.
        /// </summary>
        /// <param name="manufacturer">Название производителя.</param>
        /// <returns>Список таблеток, подходящих под фильтр.</returns>
        public List<Pill> GetPillsByManufacturer(string manufacturer)
        {
            string wanted = (manufacturer ?? string.Empty).Trim();
            return GetAllPills()
                .Where(p => string.Equals(p.Manufacturer, wanted, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Возвращает список только рецептурных препаратов.
        /// </summary>
        /// <returns>Список таблеток, отпускаемых по рецепту.</returns>
        public List<Pill> GetPrescriptionPills()
        {
            return GetAllPills().Where(p => p.IsPrescription).ToList();
        }

        /// <summary>
        /// Вычисляет общую стоимость всех запасов на складе.
        /// </summary>
        /// <returns>Сумма произведений цены на количество для всех таблеток.</returns>
        public double GetTotalStockValue()
        {
            return GetAllPills().Sum(p => p.Price * p.Quantity);
        }

        /// <summary>
        /// Возвращает таблетки, отсортированные по цене от дорогих к дешёвым.
        /// </summary>
        /// <returns>Отсортированный список таблеток.</returns>
        public List<Pill> GetPillsSortedByPriceDesc()
        {
            return GetAllPills().OrderByDescending(p => p.Price).ToList();
        }

        /// <summary>
        /// Заполняет базу тестовыми таблетками, но только если она пуста.
        /// Вызывается при старте приложений, чтобы при повторных запусках
        /// тестовые записи не дублировались.
        /// </summary>
        public void SeedDemoData()
        {
            if (_repository.ReadAll().Any())
            {
                return;
            }

            AddPill(new Pill("Парацетамол", "Фармстандарт", 150.50, 100, false));
            AddPill(new Pill("Амоксициллин", "Синтез", 450.00, 50, true));
            AddPill(new Pill("Ибупрофен", "Фармстандарт", 220.00, 80, false));
            AddPill(new Pill("Цитрамон", "Обновление", 95.90, 120, false));
            AddPill(new Pill("Азитромицин", "Синтез", 610.00, 30, true));
        }
    }
}
