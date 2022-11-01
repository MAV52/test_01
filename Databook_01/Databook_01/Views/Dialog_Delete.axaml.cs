using Avalonia.ReactiveUI;
using Databook_01.ViewModels;
using System;
using ReactiveUI;

namespace Databook_01.Views
{
    public partial class Dialog_Delete : ReactiveWindow<Dialog_DeleteViewModel>
    {
        public Dialog_Delete()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.CMD_Delete_Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.CMD_Delete_Confirm.Subscribe(Close)));
        }
    }
}
