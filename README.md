# Syhnost — «Аптека» (практическая работа №2)

Многослойное приложение: два представления (консоль и WinForms) работают с одной бизнес-логикой,
данные хранятся в базе SQL Server LocalDB.

```
ConsoleApp11 ─┐
              ├─► BusinessLogic (Logic) ─► DataAccessLayer ─► Syhnost (модель: Pill, IDomainObject)
WinForm2 ─────┘                              │
                                             ├─ IRepository<T>
                                             ├─ EntityRepository<T>  (Entity Framework, DataContext)
                                             └─ DapperRepository<T>  (Dapper)
```

## Запуск
1. Нужен SQL Server LocalDB (компонент Visual Studio «Хранение и обработка данных»). Проверка: `sqllocaldb info`.
2. Открыть `Syhnost.sln`, дождаться восстановления NuGet-пакетов.
3. Запустить `ConsoleApp11` или `WinForm2`. База `PharmacyDb` и таблица `Pills` создаются автоматически.

## Смена способа доступа к данным
В `BusinessLogic/Logic.cs`, конструктор `Logic()`:
```csharp
_repository = new EntityRepository<Pill>();   // Entity Framework
// _repository = new DapperRepository<Pill>(); // Dapper
```
Строка подключения — в `DataAccessLayer/DbSettings.cs`.
