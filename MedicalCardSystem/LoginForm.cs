using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalCardSystem
{
    public partial class LoginForm : Form
    {
        private TextBox tbLogin;
        private TextBox tbPassword;
        private Button btnLogin;

        public LoginForm()
        {
            InitializeComponent();

            // Настройка формы
            this.BackColor = Color.FromArgb(44, 62, 80);
            this.Font = new Font("Tahoma", 11);
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Авторизация - Дербентское ЦГБ";

            // Запрет на разворачивание на весь экран
            this.MaximizeBox = false;
            this.MinimizeBox = true; // Свернуть можно

            // Установка фиксированного размера (нельзя изменить размер мышью)
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Установка иконки приложения
            try
            {
                this.Icon = new Icon("logo.ico");
            }
            catch (Exception ex)
            {
                // Если иконка не найдена, просто продолжаем без нее
                System.Diagnostics.Debug.WriteLine("Иконка не загружена: " + ex.Message);
            }

            Label lblLogin = new Label() { Text = "Логин:", Location = new Point(50, 50), Size = new Size(80, 30), ForeColor = Color.White };
            tbLogin = new TextBox() { Location = new Point(140, 50), Size = new Size(180, 30), Font = new Font("Tahoma", 11) };
            Label lblPassword = new Label() { Text = "Пароль:", Location = new Point(50, 100), Size = new Size(80, 30), ForeColor = Color.White };
            tbPassword = new TextBox() { Location = new Point(140, 100), Size = new Size(180, 30), Font = new Font("Tahoma", 11), UseSystemPasswordChar = true };
            btnLogin = new Button() { Text = "Вход", Location = new Point(140, 150), Size = new Size(100, 40), BackColor = Color.FromArgb(58, 124, 165), ForeColor = Color.White, Font = new Font("Tahoma", 11, FontStyle.Bold) };
            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblLogin);
            this.Controls.Add(tbLogin);
            this.Controls.Add(lblPassword);
            this.Controls.Add(tbPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string pass = tbPassword.Text;
            var dt = DBClass.AuthenticateUser(login, pass);
            if (dt.Rows.Count > 0)
            {
                string role = dt.Rows[0]["Role"].ToString();
                int empId = Convert.ToInt32(dt.Rows[0]["IdEmployee"]);
                this.Hide();
                var mainForm = new MainForm(login, role, empId);
                mainForm.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.Font;
        }
    }
}