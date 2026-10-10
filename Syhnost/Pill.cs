namespace Syhnost
{
    /// <summary>
    /// Сущность «Таблетка».
    /// Описывает препарат в аптеке: название, производителя, цену,
    /// количество на складе и признак рецептурного отпуска.
    /// </summary>
    public class Pill : IDomainObject
    {
        /// <summary>
        /// Уникальный идентификатор таблетки.
        /// Присваивается базой данных при добавлении записи.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название препарата.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Производитель препарата.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Цена за одну упаковку.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Количество упаковок на складе.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Признак того, что препарат отпускается по рецепту.
        /// </summary>
        public bool IsPrescription { get; set; }

        /// <summary>
        /// Создаёт пустой экземпляр таблетки.
        /// Необходим для ORM (EF и Dapper создают объекты через пустой конструктор).
        /// </summary>
        public Pill() { }

        /// <summary>
        /// Создаёт новый экземпляр таблетки с заданными параметрами.
        /// </summary>
        /// <param name="name">Название препарата.</param>
        /// <param name="manufacturer">Производитель.</param>
        /// <param name="price">Цена за упаковку.</param>
        /// <param name="quantity">Количество на складе.</param>
        /// <param name="isPrescription">Признак рецептурного препарата.</param>
        public Pill(string name, string manufacturer, double price, int quantity, bool isPrescription)
        {
            Name = name;
            Manufacturer = manufacturer;
            Price = price;
            Quantity = quantity;
            IsPrescription = isPrescription;
        }
    }
}
