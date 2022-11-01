using Databook_01.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Databook_01.ViewModels
{
    public class DeleteResult   //результат при выходе из диалога для удаления записи
    {
        public bool val;    //флаг факта удаления записи
        public int ID;      //инкремент удаляемой записи
        public DeleteResult(bool code, int id)
        {
            val = code;
            ID = id;
        }
        public DeleteResult(bool code)
        {
            val = code;
            ID = -1;
        }
    }
    public class Dialog_DeleteViewModel : ViewModelBase
    {
        public Dialog_DeleteViewModel(Entry entry)
        {
            DataSelected = new();
            DataSelected.Add(entry);
            CMD_Delete_Cancel = ReactiveCommand.Create(() =>    //выходим из диалога, ничего не делаем
            {
                return new DeleteResult(false);
            });
            CMD_Delete_Confirm = ReactiveCommand.Create(() =>   //выходим из диалога, удаляем запись
            {
                return new DeleteResult(true,DataSelected[0].ID);
            });
        }
        public ReactiveCommand<Unit, DeleteResult> CMD_Delete_Cancel { get; }
        public ReactiveCommand<Unit, DeleteResult> CMD_Delete_Confirm { get; }
        public ObservableCollection<Entry> DataSelected { get; set; }   //байндинг для таблицы отображения записи
    }
}
