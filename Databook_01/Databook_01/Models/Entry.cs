using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;

// Здесь описывается структура данных и методы взаимодействия с ними

namespace Databook_01.Models
{
    public class CleanEntry //"чистая" запись без инкремента, только данные
    {
        public string? last_name { get; set; } //фамилия
        public string? first_name { get; set; } //имя
        public string? middle_name { get; set; } //отчество
        public string? phone_number { get; set; } //телефон
        public CleanEntry()
        {
            last_name = "Фамилия";
            first_name = "Имя";
            middle_name = "Отчество";
            phone_number = "Телефонный номер";
        }
    }
    public class Entry : CleanEntry //запись, непосредственно загружаемая из файла базы данных
    {
        public static readonly string[] field_name = new string[] { "Key","last_name","first_name","middle_name","phone_number"};
        public int ID { get; set; } //инкремент
        public static string Generate_SQL_Update(int ID, CleanEntry entry) //генерирует строку с данными из entry чтобы записать в строку с инкрементом ID
        {
            var res = "";
            res += String.Format("{0} = '{1}', ", Entry.field_name[1], entry.last_name);
            res += String.Format("{0} = '{1}', ", Entry.field_name[2], entry.first_name);
            res += String.Format("{0} = '{1}', ", Entry.field_name[3], entry.middle_name);
            res += String.Format("{0} = '{1}' ", Entry.field_name[4], entry.phone_number);

            res += String.Format("WHERE {0} = {1}", Entry.field_name[0], ID);
            return res;
        }
        public static string Generate_SQL_Insert(int ID, CleanEntry entry) //генерирует строку с данными из entry для добавления новой строки с инкрементом ID
        {
            var res = "";
            res += String.Format("{0},", ID);

            res += String.Format("'{0}',", entry.last_name);
            res += String.Format("'{0}',", entry.first_name);
            res += String.Format("'{0}',", entry.middle_name);
            res += String.Format("'{0}'", entry.phone_number);

            return res;
        }
        public static string Generate_SQL_Filter(Filters filter) //генерирует строку с фильтрацией данных, параметры фильтрации берутся из filter
        {
            var res = "";
            if (filter.IsEmpty()) return "1=0";
            if (filter.filter_last_name.Count > 0)          //часть с фамилиями
            {
                res += "(";
            }
            foreach (string s in filter.filter_last_name)
            {
                res += String.Format("{0} = '{1}'", Entry.field_name[1], s);
                if (filter.filter_last_name.IndexOf(s) < filter.filter_last_name.Count - 1)
                {
                    res += " OR ";
                }
                else
                {
                    res += ")";
                }
            }
            if (filter.filter_first_name.Count > 0)         //часть с именами
            {
                res += " AND (";
            }
            foreach (string s in filter.filter_first_name)
            {
                res += String.Format("{0} = '{1}'", Entry.field_name[2], s);
                if (filter.filter_first_name.IndexOf(s) < filter.filter_first_name.Count - 1)
                {
                    res += " OR ";
                }
                else
                {
                    res += ")";
                }
            }
            if (filter.filter_middle_name.Count > 0)        //часть с отчествами
            {
                res += " AND (";
            }
            foreach (string s in filter.filter_middle_name)
            {
                res += String.Format("{0} = '{1}'", Entry.field_name[3], s);
                if (filter.filter_middle_name.IndexOf(s) < filter.filter_middle_name.Count - 1)
                {
                    res += " OR ";
                }
                else
                {
                    res += ")";
                }
            }
            if (filter.filter_phone_number.Count > 0)       //часть с телефонами
            {
                res += " AND (";
            }
            foreach (string s in filter.filter_phone_number)
            {
                res += String.Format("{0} = '{1}'", Entry.field_name[4], s);
                if (filter.filter_phone_number.IndexOf(s) < filter.filter_phone_number.Count - 1)
                {
                    res += " OR ";
                }
                else
                {
                    res += ")";
                }
            }
            if (res.Length == 0) res = "1=0";
            Debug.WriteLine(res);
            return res;
        }

        public CleanEntry ToClean() //создает "чистую" запись с данными на основе текущих данных
        {
            CleanEntry res = new();
            res.last_name = this.last_name;
            res.first_name = this.first_name;
            res.middle_name = this.middle_name;
            res.phone_number = this.phone_number;
            return res;
        }
        public void Fill(DbDataReader source) //наполняет запись данными из ридера базы данных
        {
            ID = source.GetInt32(0);
            last_name = source.GetString(1);
            first_name = source.GetString(2);
            middle_name = source.GetString(3);
            phone_number = source.GetString(4);
        }
        public static bool IsCompatible(DbDataReader source) //проверяет соответствие загружаемой таблицы формату данных, описанному выше
        {
            var i = 0;
            while (source.Read())
            {
                if (i >= 5) return false;
                if (source.GetValue(0).ToString() != Entry.field_name[i])
                {
                    return false;
                }
                i++;
            }
            return true;
        }
    }
    public class Filters    //класс для фильтрации данных
    {
        public ObservableCollection<string> filter_last_name;       //массив из фамилий
        public ObservableCollection<string> filter_first_name;      //массив из имен
        public ObservableCollection<string> filter_middle_name;     //массив из отчеств
        public ObservableCollection<string> filter_phone_number;    //массив из телефонов
        public bool Updated;    //флажок апдейта, используется при возвращении из диалога с фильтрами, чтобы не посылать лишний запрос в базу
        public Filters()
        {
            filter_last_name = new();
            filter_first_name = new();
            filter_middle_name = new();
            filter_phone_number = new();
            Updated = false;
        }
        public bool IsEmpty()   //проверка на пустоту
        {
            return filter_last_name.Count == 0
                && filter_first_name.Count == 0
                && filter_middle_name.Count == 0
                && filter_phone_number.Count == 0;
        }
        public void Clear() //очистить фильтр
        {
            filter_last_name.Clear();
            filter_first_name.Clear();
            filter_middle_name.Clear();
            filter_phone_number.Clear();
        }
    }
}
