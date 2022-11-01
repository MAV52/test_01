using Avalonia.ReactiveUI;
using Databook_01.ViewModels;
using System;
using ReactiveUI;

namespace Databook_01.Views
{
    public partial class Dialog_Add : ReactiveWindow<Dialog_AddViewModel>
    {
        public Dialog_Add()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.CMD_Add_Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.CMD_Add_Confirm.Subscribe(Close)));
        }
    }
}
