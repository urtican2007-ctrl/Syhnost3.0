using System.Globalization;
using BusinessLogic;
using Syhnost;

namespace WinForm2
{
    /// <summary>
    /// Форма добавления и редактирования таблетки.
    /// Если в конструктор передана таблетка — форма работает в режиме редактирования,
    /// иначе — в режиме добавления новой записи.
    /// </summary>
    public partial class AddEditPillForm : Form
    {
        /// <summary>
        /// Ссылка на объект бизнес-логики.
        /// </summary>
        private readonly Logic _logic;

        /// <summary>
        /// Редактируемая таблетка (null — режим добавления).
        /// </summary>
        private readonly Pill? _currentPill;

        /// <summary>
        /// Поле ввода названия.
        /// </summary>
        private TextBox txtName = null!;

        /// <summary>
        /// Поле ввода производителя.
        /// </summary>
        private TextBox txtManufacturer = null!;

        /// <summary>
        /// Поле ввода цены.
        /// </summary>
        private TextBox txtPrice = null!;

        /// <summary>
        /// Поле ввода количества.
        /// </summary>
        private TextBox txtQuantity = null!;

        /// <summary>
        /// Флажок «По рецепту».
        /// </summary>
        private CheckBox chkIsPrescription = null!;

        /// <summary>
        /// Создаёт форму в режиме добавления новой таблетки.
        /// </summary>
        /// <param name="logic">Объект бизнес-логики.</param>
        public AddEditPillForm(Logic logic) : this(logic, null) { }

        /// <summary>
        /// Создаёт форму в режиме редактирования (или добавления, если pill = null).
        /// </summary>
        /// <param name="logic">Объект бизнес-логики.</param>
        /// <param name="pill">Редактируемая таблетка или null.</param>
        public AddEditPillForm(Logic logic, Pill? pill)
        {
            _logic = logic;
            _currentPill = pill;

            InitializeCustomUI();

            if (pill != null)
            {
                this.Text = "Редактирование таблетки";
                txtName.Text = pill.Name;
                txtManufacturer.Text = pill.Manufacturer;
                txtPrice.Text = pill.Price.ToString(CultureInfo.InvariantCulture);
                txtQuantity.Text = pill.Quantity.ToString();
                chkIsPrescription.Checked = pill.IsPrescription;
            }
            else
            {
                this.Text = "Добавление таблетки";
            }
        }

        /// <summary>
        /// Создаёт элементы управления формы: поля ввода, флажок и кнопку «Сохранить».
        /// </summary>
        private void InitializeCustomUI()
        {
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.BackColor = Color.White;

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(20)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            AddInputField(layout, "Название:", out txtName, 0);
            AddInputField(layout, "Производитель:", out txtManufacturer, 1);
            AddInputField(layout, "Цена:", out txtPrice, 2);
            AddInputField(layout, "Количество:", out txtQuantity, 3);

            chkIsPrescription = new CheckBox
            {
                Text = "По рецепту",
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 10)
            };
            layout.Controls.Add(new Label(), 0, 4);
            layout.Controls.Add(chkIsPrescription, 1, 4);

            Button btnSave = new Button
            {
                Text = "Сохранить",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                Height = 40,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;
            layout.Controls.Add(new Label(), 0, 5);
            layout.Controls.Add(btnSave, 1, 5);

            this.Controls.Add(layout);
        }

        /// <summary>
        /// Добавляет в таблицу подпись и поле ввода.
        /// </summary>
        /// <param name="panel">Контейнер, в который добавляются элементы.</param>
        /// <param name="labelText">Текст подписи.</param>
        /// <param name="textBox">Созданное поле ввода.</param>
        /// <param name="row">Номер строки в контейнере.</param>
        private void AddInputField(TableLayoutPanel panel, string labelText, out TextBox textBox, int row)
        {
            Label lbl = new Label { Text = labelText, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 5) };
            textBox = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 5) };
            panel.Controls.Add(lbl, 0, row);
            panel.Controls.Add(textBox, 1, row);
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки «Сохранить»: проверяет введённые данные
        /// и добавляет новую таблетку либо изменяет существующую.
        /// </summary>
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string manufacturer = txtManufacturer.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(manufacturer))
            {
                MessageBox.Show("Заполните название и производителя!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Допускаем и точку, и запятую в качестве разделителя
            string priceText = txtPrice.Text.Trim().Replace(',', '.');
            if (!double.TryParse(priceText, NumberStyles.Float, CultureInfo.InvariantCulture, out double price)
                || !int.TryParse(txtQuantity.Text.Trim(), out int quantity))
            {
                MessageBox.Show("Цена и количество должны быть числами!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.IsFinite(price) || price < 0 || quantity < 0)
            {
                MessageBox.Show("Цена и количество не могут быть отрицательными!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Pill pillData = new Pill(name, manufacturer, price, quantity, chkIsPrescription.Checked);

                if (_currentPill == null)
                {
                    _logic.AddPill(pillData);
                    MessageBox.Show("Таблетка успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    pillData.Id = _currentPill.Id;
                    _logic.UpdatePill(pillData);
                    MessageBox.Show("Изменено успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при работе с базой данных:\n" + ex.GetBaseException().Message,
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
