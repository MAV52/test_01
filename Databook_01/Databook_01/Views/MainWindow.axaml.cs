using Databook_01.Models;
using Databook_01.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Threading.Tasks;
namespace Databook_01.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowDialog_Delete.RegisterHandler(DoShowDialog_DeleteAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowDialog_Edit.RegisterHandler(DoShowDialog_EditAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowDialog_Add.RegisterHandler(DoShowDialog_AddAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowDialog_Error.RegisterHandler(DoShowErrorAsync)));
            this.WhenActivated(d => d(ViewModel!.ShowDialog_Filter.RegisterHandler(DoShowDialog_FilterAsync)));
        }
        private async Task DoShowDialog_DeleteAsync(InteractionContext<Dialog_DeleteViewModel, DeleteResult?> interaction)
        {
            var dialog = new Dialog_Delete();
            dialog.DataContext = interaction.Input;

            var res = await dialog.ShowDialog<DeleteResult?>(this);
            interaction.SetOutput(res);
        }
        private async Task DoShowDialog_EditAsync(InteractionContext<Dialog_EditViewModel, EditResult?> interaction)
        {
            var dialog = new Dialog_Edit();
            dialog.DataContext = interaction.Input;

            var res = await dialog.ShowDialog<EditResult?>(this);
            interaction.SetOutput(res);
        }
        private async Task DoShowDialog_AddAsync(InteractionContext<Dialog_AddViewModel, AddResult?> interaction)
        {
            var dialog = new Dialog_Add();
            dialog.DataContext = interaction.Input;

            var res = await dialog.ShowDialog<AddResult?>(this);
            interaction.SetOutput(res);
        }
        private async Task DoShowErrorAsync(InteractionContext<Dialog_ErrorViewModel, ErrorOK?> interaction)
        {
            var dialog = new Dialog_Error();
            dialog.DataContext = interaction.Input;

            var res = await dialog.ShowDialog<ErrorOK?>(this);
            interaction.SetOutput(res);
        }
        private async Task DoShowDialog_FilterAsync(InteractionContext<Dialog_FilterViewModel, Filters?> interaction)
        {
            var dialog = new Dialog_Filter();
            dialog.DataContext = interaction.Input;

            var res = await dialog.ShowDialog<Filters?>(this);
            interaction.SetOutput(res);
        }
    }
}
