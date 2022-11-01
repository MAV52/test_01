using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databook_01.ViewModels
{
    public class FilterItemViewModel : ViewModelBase    //VM галочки для диалога фильтров
    {
        public string Filter_Content { get; set; } 
        public bool Filter_Active { get; set; }
        public FilterItemViewModel(string s,bool b)
        {
            Filter_Content = s;
            Filter_Active = b;
        }
    }
}
