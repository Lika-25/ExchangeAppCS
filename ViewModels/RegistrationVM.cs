using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Exchange_appl.ViewModels
{
    public class RegistrationVM : ViewModelBase
    {
        public string PhoneNumber
        {
            get { return PhoneNumber; }
            set
            {
                PhoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }
        public string Password
        {
            get
            {
                return Password;
            }
            set
            {
                Password = value;
                OnPropertyChanged("Password");
            }
        }
        public string RepeatedPassword
        {
            get
            {
                return RepeatedPassword;
            }
            set
            {
                RepeatedPassword = value;
                OnPropertyChanged("RepeatedPassword");
            }
        }
        public ICommand? RegisterCommand { get; set; }

    }
}
