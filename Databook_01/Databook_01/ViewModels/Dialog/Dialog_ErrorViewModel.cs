using ReactiveUI;
using System.Reactive;

namespace Databook_01.ViewModels
{
    public class ErrorOK { }
    public class Dialog_ErrorViewModel : ViewModelBase      //окно с сообщением об ошибке
    {
        public Dialog_ErrorViewModel(string msg)
        {
            Error_Message = msg;
            CMD_Error_OK = ReactiveCommand.Create(() =>
            {
                return new ErrorOK();
            });
        }
        public ReactiveCommand<Unit, ErrorOK> CMD_Error_OK { get; }
        public string Error_Message { get; }
    }

}
