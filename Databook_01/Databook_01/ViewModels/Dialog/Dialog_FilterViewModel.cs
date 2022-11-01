using Databook_01.Models;
using Databook_01.Views;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Databook_01.ViewModels
{
    public class Dialog_FilterViewModel
    {
        public Filters filter_base;     //"базовый" фильтр, показывает всю таблицу
        public Filters filter_cur;      //текущий фильтр, с которым мы вошли в диалог
        public Filters filter_new;      //новый фильтр, с которым мы собираемся выйти из диалога 
        public Dialog_FilterViewModel(Filters fltr_base, Filters fltr_cur)
        {
            filter_base = fltr_base;
            filter_cur = fltr_cur;
            filter_new = new Filters();

            //создаем галочки для фильтра:
            Filter_last_name = new ObservableCollection<FilterItemViewModel>();
            foreach (string s in filter_base.filter_last_name)
                Filter_last_name.Add(new FilterItemViewModel(s, filter_cur.filter_last_name.Contains(s)));
            Filter_first_name = new ObservableCollection<FilterItemViewModel>();
            foreach (string s in filter_base.filter_first_name)
                Filter_first_name.Add(new FilterItemViewModel(s, filter_cur.filter_first_name.Contains(s)));
            Filter_middle_name = new ObservableCollection<FilterItemViewModel>();
            foreach (string s in filter_base.filter_middle_name)
                Filter_middle_name.Add(new FilterItemViewModel(s, filter_cur.filter_middle_name.Contains(s)));
            Filter_phone_number = new ObservableCollection<FilterItemViewModel>();
            foreach (string s in filter_base.filter_phone_number)
                Filter_phone_number.Add(new FilterItemViewModel(s, filter_cur.filter_phone_number.Contains(s)));
            
            CMD_Filter_Cancel = ReactiveCommand.Create(() =>    //выходим со старым фильтром, ничего не делаем
            {
                return filter_cur;
            });
            CMD_Filter_Confirm = ReactiveCommand.Create(() =>   //выходим в новым фильтром для дальнейшего применения
            {
                //строим фильтр из отмеченных галочек
                filter_new.Clear();
                filter_new.Updated = true;
                foreach (FilterItemViewModel check in Filter_last_name)
                {
                    if (check.Filter_Active)
                    {
                        filter_new.filter_last_name.Add(check.Filter_Content);
                    }
                }
                foreach (FilterItemViewModel check in Filter_first_name)
                {
                    if (check.Filter_Active)
                    {
                        filter_new.filter_first_name.Add(check.Filter_Content);
                    }
                }
                foreach (FilterItemViewModel check in Filter_middle_name)
                {
                    if (check.Filter_Active)
                    {
                        filter_new.filter_middle_name.Add(check.Filter_Content);
                    }
                }
                foreach (FilterItemViewModel check in Filter_phone_number)
                {
                    if (check.Filter_Active)
                    {
                        filter_new.filter_phone_number.Add(check.Filter_Content);
                    }
                }
                return filter_new;
            });
        }
        public ReactiveCommand<Unit, Filters> CMD_Filter_Cancel { get; }
        public ReactiveCommand<Unit, Filters> CMD_Filter_Confirm { get; }
        public ObservableCollection<FilterItemViewModel> Filter_last_name { get; }
        public ObservableCollection<FilterItemViewModel> Filter_first_name { get; }
        public ObservableCollection<FilterItemViewModel> Filter_middle_name { get; }
        public ObservableCollection<FilterItemViewModel> Filter_phone_number { get; }
    }
}
