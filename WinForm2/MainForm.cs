using BusinessLogic;

namespace WinForm2
{
    /// <summary>
    /// Главная форма Windows Forms приложения «Аптека».
    /// Предоставляет доступ к основным действиям: добавлению таблеток,
    /// просмотру списка и выходу из приложения.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Ссылка на объект бизнес-логики, общий для всех форм приложения.
        /// </summary>
        private readonly Logic _logic;

        /// <summary>
        /// Создаёт главную форму приложения, инициализирует бизнес-логику
        /// (подключение к базе данных) и добавляет тестовые записи,
        /// если база данных пуста.
        /// </summary>
        public MainForm()
        {
            _logic = new Logic();
            _logic.SeedDemoData();
            InitializeCustomUI();
        }

        /// <summary>
        /// Инициализирует пользовательский интерфейс главной формы:
        /// создаёт кнопки меню и размещает их в табличном контейнере.
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Text = "Аптека - Главное меню";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 12F, FontStyle.Regular);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(20)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

            Button btnAdd = CreateStyledButton("Добавить таблетку", Color.FromArgb(40, 167, 69));
            btnAdd.Click += (s, e) => new AddEditPillForm(_logic).ShowDialog();

            Button btnList = CreateStyledButton("Список таблеток", Color.FromArgb(0, 123, 255));
            btnList.Click += (s, e) => new PillListForm(_logic).ShowDialog();

            Button btnExit = CreateStyledButton("Выход", Color.FromArgb(220, 53, 69));
            btnExit.Click += (s, e) => Application.Exit();

            layout.Controls.Add(btnAdd, 0, 0);
            layout.Controls.Add(btnList, 0, 1);
            layout.Controls.Add(btnExit, 0, 2);

            this.Controls.Add(layout);
        }

        /// <summary>
        /// Создаёт кнопку с единым стилем оформления.
        /// </summary>
        /// <param name="text">Текст на кнопке.</param>
        /// <param name="backColor">Цвет фона кнопки.</param>
        /// <returns>Настроенный экземпляр кнопки.</returns>
        private Button CreateStyledButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(10)
            };
        }
    }
}
