namespace WinForm2
{
    /// <summary>
    /// Точка входа Windows Forms приложения «Аптека».
    /// Настраивает приложение и запускает главную форму.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Главный метод приложения.
        /// Инициализирует конфигурацию WinForms и открывает главную форму.
        /// Если не удалось подключиться к базе данных, показывает понятное сообщение.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            MainForm mainForm;
            try
            {
                mainForm = new MainForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось подключиться к базе данных.\n" +
                    "Проверьте, что установлен SQL Server LocalDB (компонент Visual Studio).\n\n" +
                    "Причина: " + ex.GetBaseException().Message,
                    "Ошибка подключения к БД",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(mainForm);
        }
    }
}
