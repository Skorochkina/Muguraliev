using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalCardSystem
{
    public partial class MainForm : Form
    {
        private TabControl tabControl;
        private TabPage tabPagePatients;
        private TabPage tabPageCard;
        private TabPage tabPageAppointment;
        private TabPage tabPageUsers;

        private DataGridView dgvPatients;
        private TextBox tbSearch;
        private Button btnSearch;
        private Button btnSelectPatient;

        private DataGridView dgvHistory;
        private Label lblPatientName;
        private Label lblPatientBirth;
        private Label lblPatientPhone;

        private ComboBox cbDoctors;
        private DateTimePicker dtpDate;
        private DateTimePicker dtpTime;
        private TextBox tbComplaints;
        private TextBox tbAnamnesis;
        private TextBox tbDiagnosis;
        private TextBox tbPrescriptions;
        private TextBox tbRecommendations;
        private Button btnSaveAppointment;
        private Button btnPrintRecipe;
        private Button btnPrintCertificate;

        private string currentUserRole;
        private int currentUserId;
        private int currentPatientId = -1;
        private string currentPatientName = "";

        public MainForm(string login, string role, int empId)
        {
            currentUserRole = role;
            currentUserId = empId;

            InitializeComponent();

            this.MaximizeBox = false;
            this.MinimizeBox = true;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            try
            {
                this.Icon = new Icon("logo.ico");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Иконка не загружена: " + ex.Message);
            }

            InitializeCustomTabs();
            LoadPatients();
            ConfigureAccessByRole();
            AddLogoutButton();

            this.Text = $"Медицинская информационная система - Дербентское ЦГБ - {login} ({role})";
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.Font = new Font("Tahoma", 11);
        }

        private void InitializeCustomTabs()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Tahoma", 11, FontStyle.Regular);

            tabPagePatients = new TabPage("Пациенты");
            tabPagePatients.BackColor = Color.FromArgb(240, 244, 248);

            tabPageCard = new TabPage("Карта пациента");
            tabPageCard.BackColor = Color.FromArgb(240, 244, 248);

            tabPageAppointment = new TabPage("Прием");
            tabPageAppointment.BackColor = Color.FromArgb(240, 244, 248);

            tabControl.Controls.AddRange(new Control[] { tabPagePatients, tabPageCard, tabPageAppointment });
            this.Controls.Add(tabControl);

            var panelSearch = new Panel() { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            Label lblSearch = new Label() { Text = "Поиск (Фамилия или полис):", Location = new Point(10, 18), Size = new Size(200, 30), Font = new Font("Tahoma", 11) };
            tbSearch = new TextBox() { Location = new Point(210, 15), Size = new Size(250, 30), Font = new Font("Tahoma", 11) };
            btnSearch = new Button() { Text = "Найти", Location = new Point(470, 14), Size = new Size(100, 35), BackColor = Color.FromArgb(58, 124, 165), ForeColor = Color.White, Font = new Font("Tahoma", 10, FontStyle.Bold) };
            btnSearch.Click += (s, e) => LoadPatients(tbSearch.Text);
            panelSearch.Controls.AddRange(new Control[] { lblSearch, tbSearch, btnSearch });

            dgvPatients = new DataGridView() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, Font = new Font("Tahoma", 10) };
            dgvPatients.CellDoubleClick += (s, e) => SelectPatient();

            btnSelectPatient = new Button() { Text = "Выбрать пациента", Dock = DockStyle.Bottom, Height = 45, BackColor = Color.FromArgb(58, 124, 165), ForeColor = Color.White, Font = new Font("Tahoma", 12, FontStyle.Bold) };
            btnSelectPatient.Click += (s, e) => SelectPatient();

            tabPagePatients.Controls.Add(dgvPatients);
            tabPagePatients.Controls.Add(btnSelectPatient);
            tabPagePatients.Controls.Add(panelSearch);

            var panelInfo = new Panel() { Dock = DockStyle.Top, Height = 120, Padding = new Padding(10) };
            lblPatientName = new Label() { Text = "Пациент: не выбран", Font = new Font("Tahoma", 12, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80) };
            lblPatientName.Dock = DockStyle.Top;
            lblPatientBirth = new Label() { Text = "Дата рождения: ", Font = new Font("Tahoma", 11), Dock = DockStyle.Top };
            lblPatientPhone = new Label() { Text = "Телефон: ", Font = new Font("Tahoma", 11), Dock = DockStyle.Top };
            panelInfo.Controls.Add(lblPatientPhone);
            panelInfo.Controls.Add(lblPatientBirth);
            panelInfo.Controls.Add(lblPatientName);

            dgvHistory = new DataGridView() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, Font = new Font("Tahoma", 10) };
            tabPageCard.Controls.Add(dgvHistory);
            tabPageCard.Controls.Add(panelInfo);

            var mainPanel = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 9, Padding = new Padding(10), AutoSize = true };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            mainPanel.Controls.Add(new Label() { Text = "Врач:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            cbDoctors = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 11), Height = 35 };
            mainPanel.Controls.Add(cbDoctors, 1, 0);

            mainPanel.Controls.Add(new Label() { Text = "Дата:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            dtpDate = new DateTimePicker() { Format = DateTimePickerFormat.Short, Font = new Font("Tahoma", 11), Value = DateTime.Now };
            mainPanel.Controls.Add(dtpDate, 1, 1);

            mainPanel.Controls.Add(new Label() { Text = "Время:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            dtpTime = new DateTimePicker() { Format = DateTimePickerFormat.Time, ShowUpDown = true, Font = new Font("Tahoma", 11), Value = DateTime.Now };
            mainPanel.Controls.Add(dtpTime, 1, 2);

            mainPanel.Controls.Add(new Label() { Text = "Жалобы:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.TopRight }, 0, 3);
            tbComplaints = new TextBox() { Multiline = true, Height = 60, Font = new Font("Tahoma", 11) };
            mainPanel.Controls.Add(tbComplaints, 1, 3);

            mainPanel.Controls.Add(new Label() { Text = "Анамнез:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.TopRight }, 0, 4);
            tbAnamnesis = new TextBox() { Multiline = true, Height = 60, Font = new Font("Tahoma", 11) };
            mainPanel.Controls.Add(tbAnamnesis, 1, 4);

            mainPanel.Controls.Add(new Label() { Text = "Диагноз:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.TopRight }, 0, 5);
            tbDiagnosis = new TextBox() { Multiline = true, Height = 50, Font = new Font("Tahoma", 11) };
            mainPanel.Controls.Add(tbDiagnosis, 1, 5);

            mainPanel.Controls.Add(new Label() { Text = "Назначения:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.TopRight }, 0, 6);
            tbPrescriptions = new TextBox() { Multiline = true, Height = 60, Font = new Font("Tahoma", 11) };
            mainPanel.Controls.Add(tbPrescriptions, 1, 6);

            mainPanel.Controls.Add(new Label() { Text = "Рекомендации:", Font = new Font("Tahoma", 11), TextAlign = ContentAlignment.TopRight }, 0, 7);
            tbRecommendations = new TextBox() { Multiline = true, Height = 60, Font = new Font("Tahoma", 11) };
            mainPanel.Controls.Add(tbRecommendations, 1, 7);

            var panelButtons = new FlowLayoutPanel() { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(10) };
            btnSaveAppointment = new Button() { Text = "Сохранить прием", Size = new Size(150, 40), BackColor = Color.FromArgb(58, 124, 165), ForeColor = Color.White, Font = new Font("Tahoma", 11, FontStyle.Bold) };
            btnPrintRecipe = new Button() { Text = "Рецепт", Size = new Size(120, 40), BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White, Font = new Font("Tahoma", 11) };
            btnPrintCertificate = new Button() { Text = "Справка", Size = new Size(120, 40), BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White, Font = new Font("Tahoma", 11) };

            btnSaveAppointment.Click += BtnSaveAppointment_Click;
            btnPrintRecipe.Click += (s, e) => PrintRecipe();
            btnPrintCertificate.Click += (s, e) => PrintCertificate();

            panelButtons.Controls.Add(btnSaveAppointment);
            panelButtons.Controls.Add(btnPrintRecipe);
            panelButtons.Controls.Add(btnPrintCertificate);
            mainPanel.Controls.Add(panelButtons, 1, 8);

            var backButton = new Button() { Text = "← Назад к списку пациентов", Dock = DockStyle.Bottom, Height = 40, BackColor = Color.LightGray };
            backButton.Click += (s, e) => tabControl.SelectedTab = tabPagePatients;
            mainPanel.Controls.Add(backButton, 1, 8);

            tabPageAppointment.Controls.Add(mainPanel);

            LoadDoctors();
        }

        private void PrintRecipe()
        {
            if (string.IsNullOrWhiteSpace(tbPrescriptions.Text))
            {
                MessageBox.Show("Нет назначений для печати рецепта!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show($"РЕЦЕПТ\n\nПациент: {currentPatientName}\nНазначения: {tbPrescriptions.Text}\n\nВрач: {cbDoctors.Text}\nДата: {dtpDate.Value.ToShortDateString()}",
                "Рецепт", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintCertificate()
        {
            if (string.IsNullOrWhiteSpace(tbDiagnosis.Text))
            {
                MessageBox.Show("Нет диагноза для печати справки!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show($"СПРАВКА\n\nПациент: {currentPatientName}\nДиагноз: {tbDiagnosis.Text}\nРекомендации: {tbRecommendations.Text}\n\nДата выдачи: {DateTime.Now.ToShortDateString()}",
                "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ConfigureAccessByRole()
        {
            switch (currentUserRole)
            {
                case "registrar":
                    tabPageAppointment.Enabled = false;
                    btnSaveAppointment.Enabled = false;
                    btnPrintRecipe.Enabled = false;
                    btnPrintCertificate.Enabled = false;
                    cbDoctors.Enabled = false;
                    break;

                case "doctor":
                    tabPageAppointment.Enabled = true;
                    btnSaveAppointment.Enabled = true;
                    btnPrintRecipe.Enabled = true;
                    btnPrintCertificate.Enabled = true;
                    cbDoctors.Enabled = false;
                    break;

                case "admin":
                    tabPageAppointment.Enabled = true;
                    btnSaveAppointment.Enabled = true;
                    btnPrintRecipe.Enabled = true;
                    btnPrintCertificate.Enabled = true;
                    cbDoctors.Enabled = true;
                    AddUsersTab();
                    break;
            }
        }

        private void AddUsersTab()
        {
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Text == "Пользователи")
                    return;
            }

            tabPageUsers = new TabPage("Пользователи");
            tabPageUsers.BackColor = Color.FromArgb(240, 244, 248);

            var dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Font = new Font("Tahoma", 10)
            };

            var dt = DBClass.GetAllUsers();
            dgvUsers.DataSource = dt;

            tabPageUsers.Controls.Add(dgvUsers);
            tabControl.TabPages.Add(tabPageUsers);
        }

        private void AddLogoutButton()
        {
            var btnLogout = new Button
            {
                Text = "Выйти",
                Size = new Size(100, 35),
                Location = new Point(this.ClientSize.Width - 120, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Tahoma", 10, FontStyle.Bold)
            };
            btnLogout.Click += (s, e) =>
            {
                this.Close();
                Application.Restart();
            };
            this.Controls.Add(btnLogout);
            btnLogout.BringToFront();
        }

        private void LoadDoctors()
        {
            var dt = DBClass.GetDoctors();
            cbDoctors.DisplayMember = "LastName";
            cbDoctors.ValueMember = "IdEmployee";
            cbDoctors.DataSource = dt;

            if (currentUserRole == "doctor" && cbDoctors.Items.Count > 0)
            {
                for (int i = 0; i < cbDoctors.Items.Count; i++)
                {
                    var row = ((DataRowView)cbDoctors.Items[i]).Row;
                    if (Convert.ToInt32(row["IdEmployee"]) == currentUserId)
                    {
                        cbDoctors.SelectedIndex = i;
                        break;
                    }
                }
                cbDoctors.Enabled = false;
            }
        }

        private void LoadPatients(string search = "")
        {
            var dt = DBClass.GetPatients(search);
            dgvPatients.DataSource = dt;
        }

        private void SelectPatient()
        {
            if (dgvPatients.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пациента из списка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentPatientId = Convert.ToInt32(dgvPatients.SelectedRows[0].Cells["IdPatient"].Value);

            string lastName = dgvPatients.SelectedRows[0].Cells["LastName"].Value?.ToString() ?? "";
            string firstName = dgvPatients.SelectedRows[0].Cells["FirstName"].Value?.ToString() ?? "";
            string middleName = dgvPatients.SelectedRows[0].Cells["MiddleName"].Value?.ToString() ?? "";

            currentPatientName = $"{lastName} {firstName} {middleName}".Trim();

            lblPatientName.Text = "Пациент: " + currentPatientName;

            var patientDt = DBClass.GetPatientById(currentPatientId);
            if (patientDt.Rows.Count > 0)
            {
                lblPatientBirth.Text = "Дата рождения: " + (patientDt.Rows[0]["BirthDate"]?.ToString() ?? "не указана");
                lblPatientPhone.Text = "Телефон: " + (patientDt.Rows[0]["Phone"]?.ToString() ?? "не указан");
            }

            LoadPatientHistory();

            if (currentUserRole != "registrar")
                tabControl.SelectedTab = tabPageCard;
        }

        private void LoadPatientHistory()
        {
            var dt = DBClass.GetAppointmentsByPatient(currentPatientId);
            dgvHistory.DataSource = dt;
        }

        private void BtnSaveAppointment_Click(object sender, EventArgs e)
        {
            if (currentPatientId == -1)
            {
                MessageBox.Show("Сначала выберите пациента!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedTab = tabPagePatients;
                return;
            }

            if (cbDoctors.SelectedValue == null)
            {
                MessageBox.Show("Выберите врача!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var app = new Appointment
            {
                AppointmentDate = dtpDate.Value.ToString("yyyy-MM-dd"),
                AppointmentTime = dtpTime.Value.ToString("HH:mm"),
                Complaints = tbComplaints.Text,
                Anamnesis = tbAnamnesis.Text,
                Diagnosis = tbDiagnosis.Text,
                Prescriptions = tbPrescriptions.Text,
                Recommendations = tbRecommendations.Text,
                IdPatient = currentPatientId,
                IdDoctor = Convert.ToInt32(cbDoctors.SelectedValue)
            };

            int result = DBClass.SaveAppointment(app);
            if (result > 0)
            {
                MessageBox.Show("Прием успешно сохранен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbComplaints.Clear();
                tbAnamnesis.Clear();
                tbDiagnosis.Clear();
                tbPrescriptions.Clear();
                tbRecommendations.Clear();
                LoadPatientHistory();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && btn.Text == "Выйти")
                {
                    btn.Location = new Point(this.ClientSize.Width - 120, 10);
                }
            }
        }
    }
}