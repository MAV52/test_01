using Avalonia.ReactiveUI;
using Databook_01.ViewModels;
using System;
using ReactiveUI;

namespace Databook_01.Views
{
    public partial class Dialog_Error : ReactiveWindow<Dialog_ErrorViewModel>
    {
        public Dialog_Error()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.CMD_Error_OK.Subscribe(Close)));
        }
    }
}
