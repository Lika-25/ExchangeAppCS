using Exchange_appl.Models;
using Exchange_appl.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Exchange_appl.Views
{
    public partial class ItemForm : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private DatabaseManager dbManager;
        private Models.Item _viewedItem;
        public Models.Item ViewedItem
        {
            get => _viewedItem;
            set
            {
                _viewedItem = value;
                OnPropertyChanged(nameof(ViewedItem));
            }
        }

        private bool _isEditable;
        public bool IsEditable
        {
            get => _isEditable;
            set
            {
                _isEditable = value;
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        private bool _isMyItem;
        public bool IsMyItem
        {
            get => _isMyItem;
            set
            {
                _isMyItem = value;
                OnPropertyChanged(nameof(IsMyItem));
            }
        }

        private bool _hasApplicableItems;
        public bool HasApplicableItems
        {
            get => _hasApplicableItems;
            set
            {
                _hasApplicableItems = value;
                OnPropertyChanged(nameof(HasApplicableItems));
            }
        }

        private Models.Item _selectedItem;
        public Models.Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        int? currentUserId = DatabaseManager.CurrentUserId;
   


        public ObservableCollection<Models.Item> ApplicableItems { get; set; }
        public ObservableCollection<Models.Item> FilteredApplicableItems { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<string> ExchangeCategories { get; set; }

        private MainForm _mainForm;

        public ItemForm(Models.Item viewedItem, MainForm mainForm)
        {
            InitializeComponent();
            DataContext = this; // Устанавливаем DataContext для привязки данных


            dbManager = new DatabaseManager();
            ViewedItem = viewedItem;
            _mainForm = mainForm;

            // Инициализация коллекций
            Categories = new ObservableCollection<string> { "Електроніка", "Одяг", "Взуття", "Аксесуари", "Іграшки", "Меблі", "Ремонт", "Авто", "Тварини", "Хоббі і відпочинок", "Спорт", "Інше" };
            ExchangeCategories = new ObservableCollection<string>(Categories);
            ApplicableItems = new ObservableCollection<Models.Item>(dbManager.GetApplicableItemsForExchange());
            FilteredApplicableItems = new ObservableCollection<Models.Item>(
                ApplicableItems.Where(item => item.Category == ViewedItem.ExchangeCategory));

            CheckExchangeConditions();
        }



        private string _exchangeErrorMessage;
        public string ExchangeErrorMessage
        {
            get => _exchangeErrorMessage;
            set
            {
                _exchangeErrorMessage = value;
                OnPropertyChanged(nameof(ExchangeErrorMessage));
            }
        }

        private void CheckExchangeConditions()
        {
            // Перевіряємо, чи користувач є власником речі
            IsMyItem = DatabaseManager.CurrentUserId.HasValue && DatabaseManager.CurrentUserId.Value == ViewedItem.UserId;

            // Дозволяємо редагування, якщо це річ користувача
            IsEditable = IsMyItem;

            // Оновлюємо перелік речей для обміну
            FilteredApplicableItems.Clear();
            foreach (var item in ApplicableItems.Where(item => item.Category == ViewedItem.ExchangeCategory))
            {
                FilteredApplicableItems.Add(item);
            }

            // Перевіряємо, чи є відповідні речі для обміну
            HasApplicableItems = FilteredApplicableItems.Count > 0;
            ExchangeErrorMessage = HasApplicableItems
                ? string.Empty
                : $"У вас немає товару з категорією для обміну '{ViewedItem.ExchangeCategory}'.";
        }




        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                dbManager.UpdateItem(ViewedItem);

                // Оновлюємо дані для перегляду
                var updatedItem = dbManager.GetItemById(ViewedItem.Id);


                // Синхронизация в коллекции "Мои вещи"
                var myItem = _mainForm.UserItems.FirstOrDefault(it => it.Id == updatedItem.Id);
                if (myItem != null)
                {
                    myItem.Image = updatedItem.Image;
                    myItem.ItemName = updatedItem.ItemName;
                    myItem.Description = updatedItem.Description;
                    myItem.Category = updatedItem.Category;
                    myItem.ExchangeCategory = updatedItem.ExchangeCategory;

                    _mainForm.OnPropertyChanged(nameof(_mainForm.UserItems));

                }

                MessageBox.Show("Зміни успішно збережені!");
               //ChangePhoto();

                _mainForm.OnPropertyChanged(nameof(_mainForm.UserItems));

                _mainForm.UpdateExchangeOffers();
                _mainForm.UpdateMyOffers();
                _mainForm.UpdateUserItems();
                _mainForm.LoadAllItems();
                _mainForm.LoadMyItems();
                _mainForm.LoadOffers();

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося зберегти зміни: {ex.Message}");
            }
        }

        private void ChangePhoto(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Виберіть фото"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var imagePath = openFileDialog.FileName;

                    // Оновлюємо властивість Image у ViewedItem
                    ViewedItem.Image = imagePath;

                    // Викликаємо OnPropertyChanged для оновлення Binding
                    OnPropertyChanged(nameof(ViewedItem));

                    MessageBox.Show("Фото успішно змінено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося завантажити фото: {ex.Message}");
                }
            }
        }



        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OfferClick(object sender, RoutedEventArgs e)
        {
            if (IsMyItem)
            {
                MessageBox.Show("Ви не можете обміняти свою власну річ.");
                return;
            }

            if (SelectedItem == null || !HasApplicableItems)
            {
                MessageBox.Show("У вас немає речей для обміну.");
                return;
            }

            try
            {
                // Убедитесь, что оба предмета (запрашиваемый и предложенный) существуют
                if (ViewedItem != null && SelectedItem != null)
                {
                    // Проверка на null для CurrentUserId
                    if (!DatabaseManager.CurrentUserId.HasValue)
                    {
                        MessageBox.Show("Пользователь не авторизован.");
                        return;
                    }

                    int itemOfferedId = ViewedItem.Id;
                    int itemRequestedId = SelectedItem.Id;
                    int senderId = DatabaseManager.CurrentUserId.Value;  // Убедитесь, что это значение не null
                    int receiverId = ViewedItem.UserId;  // Используем ID пользователя, который владеет предложенным предметом (ViewItem)

                    // Устанавливаем статус обмена как "Очікує" по умолчанию
                    string status = "Waiting";

                    // Добавляем запись об обмене в таблицу Exchangeoffers
                    dbManager.AddExchangeOffer(itemOfferedId, itemRequestedId, senderId, receiverId, status);

                    // Обновление интерфейса
                    MessageBox.Show("Обмін виконаний успішно!");

                    _mainForm.UpdateExchangeOffers();
                    _mainForm.UpdateMyOffers();

                    _mainForm.LoadOffers();

                    CloseWindow(sender, e); // Закрытие окна после успешного обмена
                }
                else
                {
                    MessageBox.Show("Один або обидва предмети не існують.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при обміні: {ex.Message}");
            }
        }


        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            try
            {
                // Confirm the deletion action
                var result = MessageBox.Show("Ви впевнені, що хочете видалити цей товар?", "Підтвердження", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Call the DeleteItem method from DatabaseManager to delete the item
                    dbManager.DeleteItem(ViewedItem.Id);

                    // Remove the item from the user's items collection in MainForm
                    var itemToDelete = _mainForm.UserItems.FirstOrDefault(item => item.Id == ViewedItem.Id);
                    if (itemToDelete != null)
                    {
                        _mainForm.UserItems.Remove(itemToDelete);
                    }

                    MessageBox.Show("Товар успішно видалено!");
                    _mainForm.UpdateExchangeOffers();
                    _mainForm.UpdateMyOffers();

                    _mainForm.UpdateFilteredItems();
                    _mainForm.UpdateUserItems();

                    _mainForm.OnPropertyChanged(nameof(_mainForm.UserItems));

                    _mainForm.LoadAllItems();
                    _mainForm.LoadMyItems();
                    _mainForm.LoadOffers();
                    
                  
                    CloseWindow(sender, e); // Close the window after deletion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося видалити товар: {ex.Message}");
            }
        }






    }
}
