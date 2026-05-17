using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalCardSystem
{
    // Класс "Пациент" - таблица Patient
    public class Patient
    {
        public int IdPatient { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PolicyNumber { get; set; }
        public string Snils { get; set; }

        public string GetFullName()
        {
            return $"{LastName} {FirstName} {MiddleName}".Trim();
        }
    }

    // Класс "Сотрудник" - таблица Employee
    public class Employee
    {
        public int IdEmployee { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }
        public string Specialty { get; set; }

        public string GetFullName()
        {
            return $"{LastName} {FirstName} {MiddleName}".Trim();
        }
    }

    // Класс "Пользователь" - таблица User
    public class User
    {
        public int IdUser { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int IdEmployee { get; set; }
    }

    // Класс "Прием" - таблица Appointment
    public class Appointment
    {
        public int IdAppointment { get; set; }
        public string AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string Complaints { get; set; }
        public string Anamnesis { get; set; }
        public string Diagnosis { get; set; }
        public string Prescriptions { get; set; }
        public string Recommendations { get; set; }
        public int IdPatient { get; set; }
        public int IdDoctor { get; set; }

        public string GetFullDateTime()
        {
            return $"{AppointmentDate} {AppointmentTime}";
        }
    }
}