using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange_appl.ViewModels
{
    public class AdminPanelVM : ViewModelBase
    {
        public ObservableCollection<Models.User> Users
        {
            get { return Users; }
            set
            {
                Users = value;
                OnPropertyChanged("Users");
            }
        }
        public ObservableCollection<Models.Item> Items
        {
            get { return Items; }
            set
            {
                Items = value;
                OnPropertyChanged("Items");
            }
        }
    }
}
