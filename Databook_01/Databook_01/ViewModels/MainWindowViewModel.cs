using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using ReactiveUI;
using Databook_01.Models;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Data.SQLite;
using Avalonia.Threading;
using System.Threading.Tasks;
using System.Data.Common;

namespace Databook_01.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        SQLiteConnection DB_01;
        TextBlock Table_Selected_Item;      //блок с названием выбранной таблицы
        string Table_Selected_Name;         //название выбранной таблицы
        int Entry_Selected_Item;            //номер строки выбранной записи
        Filters Filter_Base = new();        //базовый фильтр, который показывает всю таблицу
        Filters Filter_Current = new();     //текущий фильтр
        public ObservableCollection<Entry> Datalist { get; set; }   //данные с записями для датагрида
        public ObservableCollection<TextBlock> Table_List { get; set; } //список таблиц загруженной бд
        public TextBlock Table_Selected
        {
            get => Table_Selected_Item;
            set => this.RaiseAndSetIfChanged(ref Table_Selected_Item, value);
        }
        public int Entry_Selected
        {
            get => Entry_Selected_Item;
            set => this.RaiseAndSetIfChanged(ref Entry_Selected_Item, value);
        }
        public MainWindowViewModel()
        {
            ShowDialog_Delete = new Interaction<Dialog_DeleteViewModel, DeleteResult?>();
            ShowDialog_Edit = new Interaction<Dialog_EditViewModel, EditResult?>();
            ShowDialog_Add = new Interaction<Dialog_AddViewModel, AddResult?>();
            ShowDialog_Error = new Interaction<Dialog_ErrorViewModel, ErrorOK?>();
            ShowDialog_Filter = new Interaction<Dialog_FilterViewModel, Filters?>();
            Datalist = new ObservableCollection<Entry>();
            Table_List = new ObservableCollection<TextBlock>();
            Table_Selected_Item = new TextBlock();
            DB_01 = new SQLiteConnection();
            CMD_Load = ReactiveCommand.CreateFromTask(async () => { await DB_Load(); });
            CMD_Insert = ReactiveCommand.CreateFromTask(async () => { await DB_Insert(); });
            CMD_Edit = ReactiveCommand.CreateFromTask(async () => { await DB_Edit(); });
            CMD_Delete = ReactiveCommand.CreateFromTask(async () => { await DB_Delete(); });
            CMD_Filter = ReactiveCommand.CreateFromTask(async () => { await DB_Filter(); });
            this.WhenAnyValue(x => x.Table_Selected).ObserveOn(RxApp.MainThreadScheduler).Subscribe(LoadEntries!);
            this.WhenAnyValue(x => x.Entry_Selected).ObserveOn(RxApp.MainThreadScheduler).Subscribe(EntryOnClick!);

            Entry_Selected_Item = -1;
            Table_Selected_Name = "";
        }
        public ICommand CMD_Load { get; }
        public ICommand CMD_Insert { get; }
        public ICommand CMD_Edit { get; }
        public ICommand CMD_Delete { get; }
        public ICommand CMD_Filter { get; }
        public Interaction<Dialog_AddViewModel, AddResult?> ShowDialog_Add { get; }
        public Interaction<Dialog_EditViewModel, EditResult?> ShowDialog_Edit { get; }
        public Interaction<Dialog_DeleteViewModel, DeleteResult?> ShowDialog_Delete { get; }
        public Interaction<Dialog_ErrorViewModel, ErrorOK?> ShowDialog_Error { get; }
        public Interaction<Dialog_FilterViewModel, Filters?> ShowDialog_Filter { get; }
        private void LoadEntries(TextBlock? table)  //загрузка данных при клике на название таблицы в списке таблиц бд
        {
            if (table == null) return;
            Table_Selected_Name = table.Text;
            Dispatcher.UIThread.Post(() => SQL_Update_Filters(Filter_Base, Table_Selected_Name), DispatcherPriority.Background);
            Filter_Current = Filter_Base;
            Dispatcher.UIThread.Post(() => SQL_Update(Table_Selected_Name), DispatcherPriority.Background);
        }
        private async void EntryOnClick(int id)     //печать выбранной строки, чисто для дебага
        {
            await Dispatcher.UIThread.InvokeAsync(() => { Debug.WriteLine(String.Format("Selected row {0}", Entry_Selected_Item)); }, DispatcherPriority.Background);
        }
        public async Task DB_Load() //кнопка "Загрузить данные"
        {
            string[]? FileWork = await new OpenFileDialog().ShowAsync(new Window());
            if (FileWork == null) return;
            Dispatcher.UIThread.Post(() => SQL_Select_Schema(FileWork[0]), DispatcherPriority.Background);
            Dispatcher.UIThread.Post(() => SQL_Update_Filters(Filter_Base, Table_Selected_Name), DispatcherPriority.Background);
        }
        public async Task DB_Insert() //кнопка "Добавить..."
        {
            if (Table_Selected_Name.Length == 0)
            {
                await ShowError("Не выбрана таблица.");
                return;
            }
            var add_dialog = new Dialog_AddViewModel(new Entry());
            var res = await ShowDialog_Add.Handle(add_dialog);
            if (res != null && res.flag && res.value!=null)
            {
                var id = (Datalist.Count==0? 1 : Datalist[^1].ID + 1);
                var New_Values = Entry.Generate_SQL_Insert(id,res.value);
                var cmd = String.Format("INSERT INTO {0} VALUES ({1})", Table_Selected_Name, New_Values);
                Dispatcher.UIThread.Post(() => SQL_Query(cmd), DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SQL_Update_Filters(Filter_Base, Table_Selected_Name), DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SQL_Update(Table_Selected_Name, Entry.Generate_SQL_Filter(Filter_Current)), DispatcherPriority.Background);
            }
        }
        public async Task DB_Edit() //кнопка "Редактировать..."
        {
            if (Entry_Selected_Item<0 || Entry_Selected_Item>=Datalist.Count)
            {
                await ShowError("Не выбрана запись.");
                return;
            }
            var edit_dialog = new Dialog_EditViewModel(Datalist[Entry_Selected_Item]);
            var res = await ShowDialog_Edit.Handle(edit_dialog);
            if (res!=null && res.flag && res.value!=null)
            {
                var New_Values = Entry.Generate_SQL_Update(res.ID,res.value);
                var cmd = String.Format("UPDATE {0} SET {1}", Table_Selected_Name, New_Values);
                Dispatcher.UIThread.Post(() => SQL_Query(cmd), DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SQL_Update_Filters(Filter_Base, Table_Selected_Name), DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SQL_Update(Table_Selected_Name, Entry.Generate_SQL_Filter(Filter_Current)), DispatcherPriority.Background);
            }
        }
        public async Task DB_Delete()   //кнопка "Удалить..."
        {
            if (Entry_Selected_Item < 0 || Entry_Selected_Item >= Datalist.Count)
            {
                await ShowError("Не выбрана запись.");
                return;
            }
            var delete_dialog = new Dialog_DeleteViewModel(Datalist[Entry_Selected_Item]);
            var res = await ShowDialog_Delete.Handle(delete_dialog);
            if (res!=null && res.val){
                var cmd = String.Format("DELETE FROM {0} WHERE {1}={2}", Table_Selected_Name, Entry.field_name[0], Datalist[Entry_Selected_Item].ID);
                Dispatcher.UIThread.Post(() => SQL_Query(cmd), DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SQL_Update_Filters(Filter_Base, Table_Selected_Name), DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SQL_Update(Table_Selected_Name, Entry.Generate_SQL_Filter(Filter_Current)), DispatcherPriority.Background);
            }
        }
        public async Task DB_Filter()   //Кнопка "Фильтр..."
        {
            if (Table_Selected_Name.Length == 0)
            {
                await ShowError("Не выбрана таблица.");
                return;
            }
            var filter_dialog = new Dialog_FilterViewModel(Filter_Base,Filter_Current);
            var fltr = await ShowDialog_Filter.Handle(filter_dialog);
            if (fltr != null) Filter_Current = fltr;
            if (Filter_Current.Updated){
                Filter_Current.Updated = false;
                Dispatcher.UIThread.Post(() => SQL_Update(Table_Selected_Name, Entry.Generate_SQL_Filter(Filter_Current)), DispatcherPriority.Background);
            }
        }
        public async Task ShowError(string Error_msg)   //показ сообщения об ошибке
        {
            var error_dialog = new Dialog_ErrorViewModel(Error_msg);
            await ShowDialog_Error.Handle(error_dialog);
        }
        public async void SQL_Select_Schema(string FileWork)    //вытаскивание из загруженного файла списка таблиц, совместимых с нашей структурой данных
        {
            try
            {
                Datalist.Clear();
                DB_01.ConnectionString = "DataSource=" + FileWork;
                SQLiteCommand cmd = new(DB_01);
                cmd.CommandText = "SELECT name FROM sqlite_schema WHERE type='table' ORDER BY name";
                ObservableCollection<string?> table_contenders = new();
                DB_01.Open();
                Table_List.Clear();
                Entry_Selected_Item = -1;
                Table_Selected_Name = "";
                DbDataReader datareader;
                datareader = await cmd.ExecuteReaderAsync();
                while (datareader.Read())
                {
                    table_contenders.Add(datareader.GetValue(0).ToString());
                }
                datareader.Close();
                foreach (string? table in table_contenders)
                {
                    cmd.CommandText = String.Format("SELECT name FROM PRAGMA_TABLE_INFO('{0}')", table);
                    datareader = await cmd.ExecuteReaderAsync();
                    if (Entry.IsCompatible(datareader))
                    {
                        Table_List.Add(new TextBlock());
                        Table_List[^1].Text = table;
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("Table {0} not loaded.", table));
                    }
                    datareader.Close();
                }
                DB_01.Close();
                Debug.WriteLine("Task complete.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error when loading file (inner): " + e.Message);
            }
            finally
            {
                DB_01.Close();
            }
        }
        public async void SQL_Query(string cmd)     //отправка SQL-команды в бд
        {
            try
            {
                DbDataReader datareader;
                DB_01.Open();
                try
                {
                    datareader = await new SQLiteCommand(cmd, DB_01).ExecuteReaderAsync();
                    Debug.WriteLine("Task complete.");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("-- SQL exception begin --\n" + e.Message);
                    Debug.WriteLine("--- SQL exception end ---");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                DB_01.Close();
            }
        }
        public async void SQL_Update_Filters(Filters fltr,string? table_name)   //загрузка "базового" фильтра, основываясь на содержимом таблицы
        {
            if (table_name == null || table_name.Length == 0)
            {
                fltr.Clear();
                return;
            }
            try
            {
                SQLiteCommand cmd = new(DB_01);
                DB_01.Open();
                DbDataReader datareader;
                fltr.Clear();
                cmd.CommandText = String.Format("SELECT DISTINCT {0} FROM {1}", Entry.field_name[1], table_name);
                datareader = await cmd.ExecuteReaderAsync();
                while (datareader.Read()) fltr.filter_last_name.Add(datareader.GetString(0));
                datareader.Close();
                cmd.CommandText = String.Format("SELECT DISTINCT {0} FROM {1}", Entry.field_name[2], table_name);
                datareader = await cmd.ExecuteReaderAsync();
                while (datareader.Read()) fltr.filter_first_name.Add(datareader.GetString(0));
                datareader.Close();
                cmd.CommandText = String.Format("SELECT DISTINCT {0} FROM {1}", Entry.field_name[3], table_name);
                datareader = await cmd.ExecuteReaderAsync();
                while (datareader.Read()) fltr.filter_middle_name.Add(datareader.GetString(0));
                datareader.Close();
                cmd.CommandText = String.Format("SELECT DISTINCT {0} FROM {1}", Entry.field_name[4], table_name);
                datareader = await cmd.ExecuteReaderAsync();
                while (datareader.Read()) fltr.filter_phone_number.Add(datareader.GetString(0));
                datareader.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                DB_01.Close();
            }
            return;
        }
        public async void SQL_Update(string? table_name, string? filter=null)   //SELECT-запрос в указанную таблицу, с возможностью применения фильтра
        {
            if (table_name == null || table_name.Length==0) return;
            try
            {
                SQLiteCommand cmd = new(DB_01);
                cmd.CommandText = String.Format("SELECT * FROM {0}", table_name);
                if (filter != null){
                    cmd.CommandText += " WHERE "+filter;
                }
                DB_01.Open();
                DbDataReader datareader = await cmd.ExecuteReaderAsync();
                Entry entry_ph;
                Datalist.Clear();
                while (datareader.Read())
                {
                    entry_ph = new Entry();
                    entry_ph.Fill(datareader);
                    Datalist.Add(entry_ph);
                }
                DB_01.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                DB_01.Close();
            }
            return;
        }
    }
}
