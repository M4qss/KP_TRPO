using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentIS
{
    public static class GenerateHolder
    {
        static public string[] Surnames = new string[]{"Олегов","Артёмов","Мединова","Киров","Алексеев","Владимиров","Замфхирова","Яйцеславов","Доминикова"};
        static public string[] Names_Patrs = new string[] { "С.В.", "Е.А", "Н.У.", "Ф.А.", "У.Л.", "Н.Н.", "Е.У." };
    }

    public class Staff
    {
        public int ID;
        public string FIO;
        public string BirthDate;
        public string Specialization;
        public Staff()
        {
            
        }
        public void SetRandom()
        {
            Random rnd = new Random();
            ID = rnd.Next(0, 100);
            FIO = GenerateHolder.Surnames[rnd.Next(0, GenerateHolder.Surnames.Length - 1)] + " " + GenerateHolder.Names_Patrs[rnd.Next(0, GenerateHolder.Names_Patrs.Length - 1)];
        }
    }
}
