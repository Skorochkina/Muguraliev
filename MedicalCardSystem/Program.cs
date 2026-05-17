using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalCardSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Инициализация базы данных
            DBClass.InitializeDatabase();

            // Запуск формы авторизации
            Application.Run(new LoginForm());
        }
    }
}