using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Exchange_appl.ViewModels
{
    public class WelcomeVM : ViewModelBase
    {
        public ICommand? LoginCommand { get; set; }
        public ICommand? RegisterCommand { get; set; }
        public ICommand? AdminCommand { get; set; }

    }
}
