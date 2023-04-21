using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfInspect.Core;

namespace WpfInspect.ViewModels
{
    public class DetailGroupViewModel : ObservableObject
    {
        public DetailGroupViewModel(string name, IEnumerable<IDetailViewModel> details)
        {
            Name = name;
            Details = new ExtendedObservableCollection<IDetailViewModel>(details);
        }

        public string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }

        public ExtendedObservableCollection<IDetailViewModel> Details { get; set; }
    }
}
