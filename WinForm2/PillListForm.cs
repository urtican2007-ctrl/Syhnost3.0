using BusinessLogic;
using Syhnost;

namespace WinForm2
{
    /// <summary>
    /// Форма для просмотра, фильтрации, сортировки, добавления,
    /// редактирования и удаления таблеток.
    /// </summary>
    public partial class PillListForm : Form
    {
        /// <summary>
        /// Ссылка на объект бизнес-логики.
        /// </summary>
        private readonly Logic _logic;

        /// <summary>
        /// Таблица для отображения списка таблеток.
        /// </summary>
        private DataGridView dgvPills = null!;

        /// <summary>
        /// Поле ввода для фильтра по производителю.
        /// </summary>
        private TextBox txtManufacturerFilter = null!;

        /// <summary>
        /// Создаёт форму списка таблеток.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики, общий с главной формой.</param>
        public PillListForm(Logic logic)
        {
            _logic = logic;
            InitializeCustomUI();
            RefreshPillsList();
        }

        /// <summary>
        /// Инициализирует элементы управления формы:
        /// панель фильтров, панель действий и таблицу таблеток.
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Text = "Аптека - Список таблеток";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(245, 247, 250);

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(15)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            Panel leftPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            FlowLayoutPanel filterPanel = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.TopDown, AutoSize = true, WrapContents = false };

            filterPanel.Controls.Add(new Label { Text = "Фильтр по производителю:", AutoSize = true, Margin = new Padding(0, 0, 0, 5) });
            txtManufacturerFilter = new TextBox { Width = 200, Margin = new Padding(0, 0, 0, 10) };
            filterPanel.Controls.Add(txtManufacturerFilter);

            Button btnFilterMan = CreateStyledButton("Найти по производителю", Color.FromArgb(23, 162, 184));
            btnFilterMan.Click += BtnFilterMan_Click;
            filterPanel.Controls.Add(btnFilterMan);

            Button btnFilterRx = CreateStyledButton("Показать рецептурные", Color.FromArgb(23, 162, 184));
            btnFilterRx.Click += BtnFilterRx_Click;
            filterPanel.Controls.Add(btnFilterRx);

            Button btnSortPrice = CreateStyledButton("Сортировка: дорогие сначала", Color.FromArgb(255, 152, 0));
            btnSortPrice.Click += BtnSortPrice_Click;
            btnSortPrice.Margin = new Padding(0, 15, 0, 5);
            filterPanel.Controls.Add(btnSortPrice);

            Button btnRefresh = CreateStyledButton("Сбросить фильтры", Color.FromArgb(108, 117, 125));
            btnRefresh.Click += (s, e) => { txtManufacturerFilter.Clear(); RefreshPillsList(); };
            btnRefresh.Margin = new Padding(0, 15, 0, 5);
            filterPanel.Controls.Add(btnRefresh);

            leftPanel.Controls.Add(filterPanel);

            Panel rightPanel = new Panel { Dock = DockStyle.Fill };
            FlowLayoutPanel actionPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, FlowDirection = FlowDirection.LeftToRight };

            Button btnAdd = CreateStyledButton("Добавить", Color.FromArgb(40, 167, 69));
            btnAdd.Click += (s, e) => { new AddEditPillForm(_logic).ShowDialog(); RefreshPillsList(); };

            Button btnEdit = CreateStyledButton("Изменить", Color.FromArgb(0, 123, 255));
            btnEdit.Click += BtnEdit_Click;

            Button btnDelete = CreateStyledButton("Удалить", Color.FromArgb(220, 53, 69));
            btnDelete.Click += BtnDelete_Click;

            actionPanel.Controls.Add(btnAdd);
            actionPanel.Controls.Add(btnEdit);
            actionPanel.Controls.Add(btnDelete);

            dgvPills = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                MultiSelect = false,
                AutoGenerateColumns = true
            };

            rightPanel.Controls.Add(dgvPills);
            rightPanel.Controls.Add(actionPanel);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightPanel, 1, 0);

            this.Controls.Add(mainLayout);
        }

        /// <summary>
        /// Обновляет таблицу таблеток.
        /// Если передан список, отображает его; иначе загружает все записи.
        /// </summary>
        /// <param name="pillsToDisplay">Необязательный список таблеток для отображения.</param>
        private void RefreshPillsList(List<Pill>? pillsToDisplay = null)
        {
            dgvPills.DataSource = null;
            List<Pill> pills = pillsToDisplay ?? _logic.GetAllPills();
            dgvPills.DataSource = pills;

            if (dgvPills.Columns["Id"] != null) dgvPills.Columns["Id"]!.HeaderText = "ID";
            if (dgvPills.Columns["Name"] != null) dgvPills.Columns["Name"]!.HeaderText = "Название";
            if (dgvPills.Columns["Manufacturer"] != null) dgvPills.Columns["Manufacturer"]!.HeaderText = "Производитель";
            if (dgvPills.Columns["Price"] != null) dgvPills.Columns["Price"]!.HeaderText = "Цена";
            if (dgvPills.Columns["Quantity"] != null) dgvPills.Columns["Quantity"]!.HeaderText = "Кол-во";
            if (dgvPills.Columns["IsPrescription"] != null) dgvPills.Columns["IsPrescription"]!.HeaderText = "Рецепт";
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
                AutoSize = true,
                Padding = new Padding(10, 5, 10, 5),
                Margin = new Padding(0, 0, 10, 10),
                Cursor = Cursors.Hand
            };
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки поиска по производителю.
        /// </summary>
        private void BtnFilterMan_Click(object? sender, EventArgs e)
        {
            string manufacturer = txtManufacturerFilter.Text.Trim();
            if (string.IsNullOrWhiteSpace(manufacturer)) { MessageBox.Show("Введите производителя!", "Внимание"); return; }
            RefreshPillsList(_logic.GetPillsByManufacturer(manufacturer));
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки показа рецептурных препаратов.
        /// </summary>
        private void BtnFilterRx_Click(object? sender, EventArgs e)
        {
            RefreshPillsList(_logic.GetPrescriptionPills());
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки сортировки по цене.
        /// </summary>
        private void BtnSortPrice_Click(object? sender, EventArgs e)
        {
            RefreshPillsList(_logic.GetPillsSortedByPriceDesc());
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки редактирования выбранной таблетки.
        /// </summary>
        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvPills.SelectedRows.Count > 0)
            {
                Pill selectedPill = (Pill)dgvPills.SelectedRows[0].DataBoundItem!;
                new AddEditPillForm(_logic, selectedPill).ShowDialog();
                RefreshPillsList();
            }
            else MessageBox.Show("Выберите таблетку для редактирования!", "Внимание");
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки удаления выбранной таблетки.
        /// </summary>
        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvPills.SelectedRows.Count > 0)
            {
                Pill selectedPill = (Pill)dgvPills.SelectedRows[0].DataBoundItem!;
                if (MessageBox.Show($"Удалить {selectedPill.Name}?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        _logic.DeletePill(selectedPill.Id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при работе с базой данных:\n" + ex.GetBaseException().Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RefreshPillsList();
                }
            }
            else MessageBox.Show("Выберите таблетку для удаления!", "Внимание");
        }
    }
}
