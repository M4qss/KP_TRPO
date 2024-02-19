using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentIS
{
    //Перечисление прав для ролей
    public enum Permission
    {
        ChangePrice,
        OpenPatients,
        ChangePatient,
        ChangePatientCard,
        RegisterPatient,
        OpenDocument,
        GetMyDocuments,
        GetAllDocuments,
        GetAllReports,
        OpenJournal,
        AddJournalEntry,
        OpenStaff,
        AddStaff,
        ChangeStaff
    }
    public class Account
    {
        public Account(string l, string p, string r, Staff pers) { login = l; password = p; role = r; person = pers; }
        string login;
        string password;
        string role;
        Staff person;
        public Staff GetStaff() { return person; }
        public string GetRole() { return role; }
        public static bool TryFind(List<Account> accounts, string log, string pass)
        {
            Account acc = Get(accounts, log, pass);
            if (acc == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static Account Get(List<Account> accounts, string log, string pass)
        {
            return  accounts.Find(x => x.login == log && x.password == pass);
        }
        
        public List<Permission> GetPermission()
        {
            if(role == "администратор")
            {
                return new List<Permission>() { Permission.OpenPatients, Permission.RegisterPatient, Permission.ChangePatient, Permission.OpenJournal, Permission.AddJournalEntry, Permission.AddJournalEntry, Permission.OpenStaff, Permission.ChangeStaff };
            }
            else if(role == "медработник")
            {
                return new List<Permission>() { Permission.OpenDocument, Permission.GetMyDocuments, Permission.ChangePatientCard, Permission.OpenPatients, Permission.OpenJournal};
            }
            else if (role == "бухгалтер")
            {
                return new List<Permission>() { Permission.OpenDocument, Permission.GetAllDocuments, Permission.GetAllReports};
            }
            else if (role == "главврач")
            {
                return new List<Permission>() { Permission.ChangePrice };
            }
            else if (role == "тестировщик")
            {
                //Все доступные права
                return Enum.GetValues(typeof(Permission)).Cast<Permission>().ToList();
            }
            else
            {
                throw new Exception($"Ошибка получения прав. {role} - неопределенная роль");
            }
        }
    }
}
