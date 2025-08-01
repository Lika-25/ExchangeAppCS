using System.Collections.ObjectModel;
using System.Linq;

namespace Exchange_appl.ViewModels
{
    public class ItemVM : ViewModelBase
    {
        private Models.Item _viewedItem;
        public Models.Item ViewedItem
        {
            get => _viewedItem;
            set
            {
                _viewedItem = value;
                OnPropertyChanged(nameof(ViewedItem));
                OnPropertyChanged(nameof(IsMyItem));
                OnPropertyChanged(nameof(AuthorPhone));
            }
        }

        private Models.User _viewingUser;
        public Models.User ViewingUser
        {
            get => _viewingUser;
            set
            {
                _viewingUser = value;
                OnPropertyChanged(nameof(ViewingUser));
            }
        }

        public ObservableCollection<Models.Item> ApplicableItems { get; set; }
        public ObservableCollection<Models.Item> FilteredApplicableItems { get; set; }

        public bool IsMyItem => ViewingUser?.Items?.Any(it => it.Id == ViewedItem?.Id) ?? false;

        public string AuthorPhone => ViewedItem?.User?.Username ?? "No telefone";

        public ItemVM(Models.Item viewedItem, Models.User viewingUser)
        {
            ViewedItem = viewedItem;
            ViewingUser = viewingUser;

            ApplicableItems = new ObservableCollection<Models.Item>();
            FilteredApplicableItems = new ObservableCollection<Models.Item>();

            GetApplicableItems();
        }

        public void GetApplicableItems()
        {
            ApplicableItems.Clear();
            if (ViewingUser?.Items != null)
            {
                foreach (var item in ViewingUser.Items)
                {
                    ApplicableItems.Add(item);
                }
            }

            UpdateFilteredApplicableItems();
        }

        public void UpdateFilteredApplicableItems()
        {
            FilteredApplicableItems.Clear();
            foreach (var item in ApplicableItems.Where(i => i.Category == ViewedItem.ExchangeCategory))
            {
                FilteredApplicableItems.Add(item);
            }
        }

        public void UpdateViewedItem(Models.Item updatedItem)
        {
            var existingItem = ApplicableItems.FirstOrDefault(it => it.Id == updatedItem.Id);
            if (existingItem != null)
            {
                existingItem.Image = updatedItem.Image;
                existingItem.ItemName = updatedItem.ItemName;
                existingItem.Description = updatedItem.Description;
                existingItem.Category = updatedItem.Category;
                existingItem.ExchangeCategory = updatedItem.ExchangeCategory;

                // Уведомляем об изменении
                OnPropertyChanged(nameof(ApplicableItems));
            }
        }



    }
}
