using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace MedicalCardSystem
{
    public class DBClass
    {
        // Путь к файлу базы данных
        private static string dbPath = Path.Combine(Application.StartupPath, "MedicalDB.db3");
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        // Метод для получения соединения
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        // Выполнение запроса, возвращающего данные (SELECT)
        public static DataTable ExecuteQuery(string query, SQLiteParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    var dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    return dt;
                }
            }
        }

        // Выполнение запроса без возврата данных (INSERT, UPDATE, DELETE)
        public static int ExecuteNonQuery(string query, SQLiteParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Инициализация базы данных: создание таблиц, если их нет, и заполнение тестовыми данными
        // Инициализация базы данных: создание таблиц, если их нет, и заполнение тестовыми данными
        public static void InitializeDatabase()
        {
            // Создание таблиц, если они не существуют
            string createTablesQuery = @"
        CREATE TABLE IF NOT EXISTS Patient (
            IdPatient INTEGER PRIMARY KEY AUTOINCREMENT,
            LastName TEXT NOT NULL,
            FirstName TEXT NOT NULL,
            MiddleName TEXT,
            BirthDate TEXT,
            Gender TEXT,
            Address TEXT,
            Phone TEXT,
            PolicyNumber TEXT,
            Snils TEXT
        );

        CREATE TABLE IF NOT EXISTS Employee (
            IdEmployee INTEGER PRIMARY KEY AUTOINCREMENT,
            LastName TEXT NOT NULL,
            FirstName TEXT NOT NULL,
            MiddleName TEXT,
            Position TEXT,
            Specialty TEXT
        );

        CREATE TABLE IF NOT EXISTS User (
            IdUser INTEGER PRIMARY KEY AUTOINCREMENT,
            Login TEXT NOT NULL UNIQUE,
            PasswordHash TEXT NOT NULL,
            Role TEXT NOT NULL,
            IdEmployee INTEGER NOT NULL,
            FOREIGN KEY(IdEmployee) REFERENCES Employee(IdEmployee)
        );

        CREATE TABLE IF NOT EXISTS Appointment (
            IdAppointment INTEGER PRIMARY KEY AUTOINCREMENT,
            AppointmentDate TEXT NOT NULL,
            AppointmentTime TEXT NOT NULL,
            Complaints TEXT,
            Anamnesis TEXT,
            Diagnosis TEXT,
            Prescriptions TEXT,
            Recommendations TEXT,
            IdPatient INTEGER NOT NULL,
            IdDoctor INTEGER NOT NULL,
            FOREIGN KEY(IdPatient) REFERENCES Patient(IdPatient),
            FOREIGN KEY(IdDoctor) REFERENCES Employee(IdEmployee)
        );
    ";
            ExecuteNonQuery(createTablesQuery);

            // Проверяем и добавляем сотрудников
            var empCheck = ExecuteQuery("SELECT COUNT(*) FROM Employee");
            if (Convert.ToInt32(empCheck.Rows[0][0]) == 0)
            {
                // Добавляем сотрудников
                ExecuteNonQuery("INSERT INTO Employee (LastName, FirstName, MiddleName, Position, Specialty) VALUES ('Иванов', 'Иван', 'Иванович', 'Врач', 'Терапевт')");
                ExecuteNonQuery("INSERT INTO Employee (LastName, FirstName, MiddleName, Position, Specialty) VALUES ('Петрова', 'Мария', 'Сергеевна', 'Врач', 'Педиатр')");
                ExecuteNonQuery("INSERT INTO Employee (LastName, FirstName, MiddleName, Position, Specialty) VALUES ('Сидорова', 'Анна', 'Петровна', 'Регистратор', NULL)");
                ExecuteNonQuery("INSERT INTO Employee (LastName, FirstName, MiddleName, Position, Specialty) VALUES ('Администратор', 'Системный', NULL, 'Администратор', NULL)");
            }

            // Получаем ID сотрудников для правильной привязки
            var empIvanov = ExecuteQuery("SELECT IdEmployee FROM Employee WHERE LastName = 'Иванов'");
            var empPetrova = ExecuteQuery("SELECT IdEmployee FROM Employee WHERE LastName = 'Петрова'");
            var empSidorova = ExecuteQuery("SELECT IdEmployee FROM Employee WHERE LastName = 'Сидорова'");
            var empAdmin = ExecuteQuery("SELECT IdEmployee FROM Employee WHERE LastName = 'Администратор'");

            int idIvanov = empIvanov.Rows.Count > 0 ? Convert.ToInt32(empIvanov.Rows[0]["IdEmployee"]) : 1;
            int idPetrova = empPetrova.Rows.Count > 0 ? Convert.ToInt32(empPetrova.Rows[0]["IdEmployee"]) : 2;
            int idSidorova = empSidorova.Rows.Count > 0 ? Convert.ToInt32(empSidorova.Rows[0]["IdEmployee"]) : 3;
            int idAdmin = empAdmin.Rows.Count > 0 ? Convert.ToInt32(empAdmin.Rows[0]["IdEmployee"]) : 4;

            // Проверяем и добавляем пользователей
            var userCheck = ExecuteQuery("SELECT COUNT(*) FROM User");
            if (Convert.ToInt32(userCheck.Rows[0][0]) == 0)
            {
                // Врачи
                ExecuteNonQuery($"INSERT INTO User (Login, PasswordHash, Role, IdEmployee) VALUES ('ivanov', '123', 'doctor', {idIvanov})");
                ExecuteNonQuery($"INSERT INTO User (Login, PasswordHash, Role, IdEmployee) VALUES ('petrova', '123', 'doctor', {idPetrova})");
                // Регистратор
                ExecuteNonQuery($"INSERT INTO User (Login, PasswordHash, Role, IdEmployee) VALUES ('reg1', '123', 'registrar', {idSidorova})");
                // Администратор
                ExecuteNonQuery($"INSERT INTO User (Login, PasswordHash, Role, IdEmployee) VALUES ('admin', 'admin', 'admin', {idAdmin})");
            }

            // Проверяем, есть ли пациенты, если нет – добавляем тестовых пациентов
            var patientCheck = ExecuteQuery("SELECT COUNT(*) FROM Patient");
            if (Convert.ToInt32(patientCheck.Rows[0][0]) == 0)
            {
                ExecuteNonQuery("INSERT INTO Patient (LastName, FirstName, MiddleName, BirthDate, Gender, Address, Phone, PolicyNumber, Snils) VALUES ('Кузнецов', 'Дмитрий', 'Алексеевич', '1985-07-15', 'М', 'г. Дербент, ул. Ленина 10', '+7(999)123-45-67', '1234567890123456', '123-456-789 00')");
                ExecuteNonQuery("INSERT INTO Patient (LastName, FirstName, MiddleName, BirthDate, Gender, Address, Phone, PolicyNumber, Snils) VALUES ('Смирнова', 'Елена', 'Викторовна', '1992-03-22', 'Ж', 'г. Дербент, ул. Советская 5', '+7(999)234-56-78', '2345678901234567', '234-567-890 01')");
                ExecuteNonQuery("INSERT INTO Patient (LastName, FirstName, MiddleName, BirthDate, Gender, Address, Phone, PolicyNumber, Snils) VALUES ('Васильев', 'Сергей', 'Петрович', '1978-11-03', 'М', 'г. Дербент, пр. Строителей 15', '+7(999)345-67-89', '3456789012345678', '345-678-901 02')");

                // Добавляем тестовые приемы для пациентов
                ExecuteNonQuery($"INSERT INTO Appointment (AppointmentDate, AppointmentTime, Complaints, Anamnesis, Diagnosis, Prescriptions, Recommendations, IdPatient, IdDoctor) VALUES ('2025-05-10', '10:30', 'Кашель, температура 37.5', 'Болеет 3 дня', 'Острая респираторная инфекция', 'Амоксициллин 500 мг 3 раза в день, обильное питье', 'Постельный режим', 1, {idIvanov})");
                ExecuteNonQuery($"INSERT INTO Appointment (AppointmentDate, AppointmentTime, Complaints, Anamnesis, Diagnosis, Prescriptions, Recommendations, IdPatient, IdDoctor) VALUES ('2025-05-12', '14:00', 'Боль в горле при глотании', 'Началось вчера вечером', 'Тонзиллит', 'Спрей Тантум Верде, полоскание раствором соли', 'Пить теплые напитки', 2, {idPetrova})");
            }
        }

        // ---------- Методы для работы с данными ----------
        public static DataTable GetPatients(string searchText = "")
        {
            string query = "SELECT IdPatient, LastName, FirstName, MiddleName, BirthDate, Phone, PolicyNumber FROM Patient";
            if (!string.IsNullOrEmpty(searchText))
            {
                query += " WHERE LastName LIKE @search OR PolicyNumber LIKE @search";
                var param = new SQLiteParameter("@search", $"%{searchText}%");
                return ExecuteQuery(query, new[] { param });
            }
            return ExecuteQuery(query);
        }

        public static DataTable GetPatientById(int id)
        {
            string query = "SELECT * FROM Patient WHERE IdPatient = @id";
            var param = new SQLiteParameter("@id", id);
            return ExecuteQuery(query, new[] { param });
        }

        public static DataTable GetAppointmentsByPatient(int patientId)
        {
            string query = @"SELECT a.*, e.LastName || ' ' || e.FirstName || ' ' || e.MiddleName as DoctorName 
                            FROM Appointment a
                            JOIN Employee e ON a.IdDoctor = e.IdEmployee
                            WHERE a.IdPatient = @patientId
                            ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";
            var param = new SQLiteParameter("@patientId", patientId);
            return ExecuteQuery(query, new[] { param });
        }

        public static int SaveAppointment(Appointment app)
        {
            if (app.IdAppointment == 0) // INSERT
            {
                string query = @"INSERT INTO Appointment (AppointmentDate, AppointmentTime, Complaints, Anamnesis, Diagnosis, Prescriptions, Recommendations, IdPatient, IdDoctor)
                                VALUES (@date, @time, @complaints, @anamnesis, @diagnosis, @prescriptions, @recommendations, @idPatient, @idDoctor)";
                var parameters = new[]
                {
                    new SQLiteParameter("@date", app.AppointmentDate),
                    new SQLiteParameter("@time", app.AppointmentTime),
                    new SQLiteParameter("@complaints", app.Complaints ?? ""),
                    new SQLiteParameter("@anamnesis", app.Anamnesis ?? ""),
                    new SQLiteParameter("@diagnosis", app.Diagnosis ?? ""),
                    new SQLiteParameter("@prescriptions", app.Prescriptions ?? ""),
                    new SQLiteParameter("@recommendations", app.Recommendations ?? ""),
                    new SQLiteParameter("@idPatient", app.IdPatient),
                    new SQLiteParameter("@idDoctor", app.IdDoctor)
                };
                return ExecuteNonQuery(query, parameters);
            }
            else // UPDATE
            {
                string query = @"UPDATE Appointment SET AppointmentDate=@date, AppointmentTime=@time, Complaints=@complaints, 
                                Anamnesis=@anamnesis, Diagnosis=@diagnosis, Prescriptions=@prescriptions, Recommendations=@recommendations,
                                IdPatient=@idPatient, IdDoctor=@idDoctor
                                WHERE IdAppointment=@id";
                var parameters = new[]
                {
                    new SQLiteParameter("@date", app.AppointmentDate),
                    new SQLiteParameter("@time", app.AppointmentTime),
                    new SQLiteParameter("@complaints", app.Complaints ?? ""),
                    new SQLiteParameter("@anamnesis", app.Anamnesis ?? ""),
                    new SQLiteParameter("@diagnosis", app.Diagnosis ?? ""),
                    new SQLiteParameter("@prescriptions", app.Prescriptions ?? ""),
                    new SQLiteParameter("@recommendations", app.Recommendations ?? ""),
                    new SQLiteParameter("@idPatient", app.IdPatient),
                    new SQLiteParameter("@idDoctor", app.IdDoctor),
                    new SQLiteParameter("@id", app.IdAppointment)
                };
                return ExecuteNonQuery(query, parameters);
            }
        }

        public static int DeleteAppointment(int id)
        {
            string query = "DELETE FROM Appointment WHERE IdAppointment = @id";
            var param = new SQLiteParameter("@id", id);
            return ExecuteNonQuery(query, new[] { param });
        }

        public static DataTable AuthenticateUser(string login, string password)
        {
            string query = @"SELECT u.*, e.LastName, e.FirstName, e.MiddleName 
                            FROM User u
                            JOIN Employee e ON u.IdEmployee = e.IdEmployee
                            WHERE u.Login = @login AND u.PasswordHash = @password";
            var parameters = new[]
            {
                new SQLiteParameter("@login", login),
                new SQLiteParameter("@password", password)
            };
            return ExecuteQuery(query, parameters);
        }

        public static DataTable GetEmployeesByRole(string role = "doctor")
        {
            string query = @"SELECT e.* FROM Employee e
                            JOIN User u ON e.IdEmployee = u.IdEmployee
                            WHERE u.Role = @role";
            var param = new SQLiteParameter("@role", role);
            return ExecuteQuery(query, new[] { param });
        }

        public static DataTable GetDoctors()
        {
            return GetEmployeesByRole("doctor");
        }

        // Получение всех пользователей (для администратора)
        public static DataTable GetAllUsers()
        {
            string query = @"SELECT u.IdUser, u.Login, u.Role, e.LastName, e.FirstName, e.MiddleName, e.Position
                            FROM User u
                            JOIN Employee e ON u.IdEmployee = e.IdEmployee";
            return ExecuteQuery(query);
        }
    }
}