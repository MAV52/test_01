using Databook_01.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Databook_01.ViewModels
{
    public class EditResult //результат при выходе из диалога для апдейта записи
    {
        public bool flag;   //флаг факта изменения записи
        public int ID;      //инкремент изменяемой записи
        public CleanEntry? value;
        public EditResult(bool code, Entry entry)
        {
            flag = code;
            ID = entry.ID;
            value = entry.ToClean();
        }
        public EditResult(bool code, int id, CleanEntry entry)
        {
            flag = code;
            ID = id;
            value = entry;
        }
        public EditResult(bool code)
        {
            flag = code;
            value = null;
        }
    }
    public class Dialog_EditViewModel : ViewModelBase
    {
        public Dialog_EditViewModel(Entry entry)
        {
            DataSelected_old = new();  //старые значения записи
            DataSelected_old.Add(entry);
            DataSelected_new = new();  //новые значения записи
            DataSelected_new.Add(entry);
            id = entry.ID;
            CMD_Edit_Cancel = ReactiveCommand.Create(() =>  //выходим из диалога, ничего не делаем
            {
                return new EditResult(false);
            });
            CMD_Edit_Confirm = ReactiveCommand.Create(() => //выходим из диалога, апдейтим запись
            {
                return new EditResult(true,id,DataSelected_new[0]);
            });
        }
        public ReactiveCommand<Unit, EditResult> CMD_Edit_Cancel { get; }
        public ReactiveCommand<Unit, EditResult> CMD_Edit_Confirm { get; }
        public ObservableCollection<CleanEntry> DataSelected_old { get; set; }
        public ObservableCollection<CleanEntry> DataSelected_new { get; set; }
        public int id;
    }

}
