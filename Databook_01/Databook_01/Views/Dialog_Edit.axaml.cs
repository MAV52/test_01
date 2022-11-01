using Avalonia.ReactiveUI;
using Databook_01.ViewModels;
using System;
using ReactiveUI;

namespace Databook_01.Views
{
    public partial class Dialog_Edit : ReactiveWindow<Dialog_EditViewModel>
    {
        public Dialog_Edit()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.CMD_Edit_Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.CMD_Edit_Confirm.Subscribe(Close)));
        }
    }
}
