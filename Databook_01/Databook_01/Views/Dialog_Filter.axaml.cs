using Avalonia.ReactiveUI;
using Databook_01.ViewModels;
using ReactiveUI;
using System;

namespace Databook_01.Views
{
    public partial class Dialog_Filter : ReactiveWindow<Dialog_FilterViewModel>
    {
        public Dialog_Filter()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.CMD_Filter_Confirm.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.CMD_Filter_Cancel.Subscribe(Close)));
        }
    }
}
