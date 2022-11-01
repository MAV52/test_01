using Databook_01.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Databook_01.ViewModels
{
    public class AddResult  //результат при выходе из диалога для добавления записи
    {
        public bool flag;           //флаг факта добавления новой записи
        public CleanEntry? value;   //собственно данные
        public AddResult(bool code, CleanEntry entry)
        {
            flag = code;
            value = entry;
        }
        public AddResult(bool code)
        {
            flag = code;
            value = null;
        }
    }
    public class Dialog_AddViewModel : ViewModelBase
    {
        public Dialog_AddViewModel(Entry entry)
        {
            DataSelected_new = new();
            DataSelected_new.Add(entry.ToClean());          //получаем полную запись, конвертируем в "чистую" чтобы не показывать инкремент
            CMD_Add_Cancel = ReactiveCommand.Create(() =>   //Возвращаемся из диалога, ничего не делаем
            {
                return new AddResult(false);
            });
            CMD_Add_Confirm = ReactiveCommand.Create(() =>  //Возвращаемся из диалога, создаем новую запись
            {
                return new AddResult(true, DataSelected_new[0]);
            });
        }
        public ReactiveCommand<Unit, AddResult> CMD_Add_Cancel { get; }
        public ReactiveCommand<Unit, AddResult> CMD_Add_Confirm { get; }
        public ObservableCollection<CleanEntry> DataSelected_new { get; set; }  //байндинг для таблицы создания записи
    }

}
