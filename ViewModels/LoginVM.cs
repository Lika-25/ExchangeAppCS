using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Exchange_appl.ViewModels
{
    public class LoginVM : ViewModelBase
    {
        public string Phone
        {
            get { return Phone; }
            set
            {
                Phone = value;
                OnPropertyChanged("Phone");
            }
        }
        public string Password
        {
            get { return Password; }
            set
            {
                Password = value;
                OnPropertyChanged("Password");
            }
        }
       // public ICommand LoginCommand { get; set; }


    }
}
