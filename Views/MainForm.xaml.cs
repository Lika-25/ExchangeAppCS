using Exchange_appl.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Exchange_appl.Views
{
    public partial class MainForm : Window
    {
        private DatabaseManager dbManager;
        public event PropertyChangedEventHandler? PropertyChanged;


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ObservableCollection<Models.Item> _allItems;
        public ObservableCollection<Models.Item> AllItems
        {
            get => _allItems;
            set
            {
                _allItems = value;
                this.OnPropertyChanged(nameof(AllItems));
            }
        }

        public ObservableCollection<Models.Item> _filteredItems;
        public ObservableCollection<Models.Item> FilteredItems
        {
            get { return _filteredItems; }
            set
            {
                _filteredItems = value;
                OnPropertyChanged(nameof(FilteredItems));
            }
        }



        private ObservableCollection<Models.Exchangeoffer> _offersToMe = new();
        public ObservableCollection<Models.Exchangeoffer> OffersToMe
        {
            get => _offersToMe;
            set
            {
                _offersToMe = value;
                OnPropertyChanged(nameof(OffersToMe));
            }
        }

        private ObservableCollection<Models.Exchangeoffer> _myOffers = new();
        public ObservableCollection<Models.Exchangeoffer> MyOffers
        {
            get => _myOffers;
            set
            {
                _myOffers = value;
                OnPropertyChanged(nameof(MyOffers));
            }
        }


        public ObservableCollection<Models.Item> _userItems;
        public ObservableCollection<Models.Item> UserItems
        {
            get { return _userItems; }
            set
            {
                _userItems = value;
                OnPropertyChanged(nameof(UserItems));
            }
        }

        private string? _selectedCategory;
        public string? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    UpdateFilteredItems();

                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();

            this.DataContext = this;
            dbManager = new DatabaseManager();
            _allItems = new ObservableCollection<Models.Item>();
            _filteredItems = new ObservableCollection<Models.Item>();
            DataContext = this;

            UpdateExchangeOffers();
            UpdateMyOffers();

            LoadOffers();
            LoadAllItems();
            foreach (var item in _allItems)
            {
                _filteredItems.Add(item);
            }
            LoadMyItems();

            if (DatabaseManager.CurrentUserId.HasValue)
            {
                // Загружаем предложения для текущего пользователя
                _offersToMe = dbManager.GetOffersToMe(DatabaseManager.CurrentUserId.Value);

                // Загружаем мои предложения
                _myOffers = dbManager.GetMyOffers(DatabaseManager.CurrentUserId.Value); // Добавлено .Value
            }
            else
            {
                MessageBox.Show("Користувач не авторизований.");
            }
        }



    
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            dbManager.Logout(); // Очищення Id користувача при закритті форми
        }


        public void LoadAllItems()
        {
            AllItems = dbManager.GetAllItems(); // Отримання всіх даних з БД
            UpdateFilteredItems(); // Оновлення фільтрованого списку
        }


        public void LoadMyItems()
        {
            if (!DatabaseManager.CurrentUserId.HasValue)
            {
                MessageBox.Show("Будь ласка, увійдіть до системи, щоб переглядати ваші речі.");
                return;
            }

            UserItems = new ObservableCollection<Models.Item>(dbManager.GetAllUserItems(DatabaseManager.CurrentUserId.Value));
            UpdateUserItems();
        }


        public void LoadOffers()
        {
            if (!DatabaseManager.CurrentUserId.HasValue)
            {
                MessageBox.Show("Будь ласка, увійдіть до системи, щоб переглядати пропозиції.");
                return;
            }
            
            // Загружаем предложения, отправленные мне
            OffersToMe = new ObservableCollection<Models.Exchangeoffer>(dbManager.GetOffersToMe(DatabaseManager.CurrentUserId.Value));


            // Загружаем мои предложения
            MyOffers = new ObservableCollection<Models.Exchangeoffer>(dbManager.GetMyOffers(DatabaseManager.CurrentUserId.Value));
        }


        private StackPanel CreateItemControl(Models.Item item)
        {
            var itemPanel = new StackPanel { Margin = new Thickness(10) };

            // Додаємо назву
            var itemNameTextBlock = new TextBlock { Text = item.ItemName, FontWeight = FontWeights.Bold, FontSize = 18 };
            itemPanel.Children.Add(itemNameTextBlock);

            // Додаємо категорію
            var categoryTextBlock = new TextBlock { Text = $"Категорія: {item.Category}", FontSize = 16 };
            itemPanel.Children.Add(categoryTextBlock);

            // Додаємо категорію обміну
            var exchangeCategoryTextBlock = new TextBlock { Text = $"Категорія для обміну: {item.ExchangeCategory}", FontSize = 16 };
            itemPanel.Children.Add(exchangeCategoryTextBlock);

            // Додаємо зображення
            if (!string.IsNullOrEmpty(item.Image))
            {
                try
                {
                    var image = new Image
                    {
                        Source = new BitmapImage(new Uri(item.Image)),
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    itemPanel.Children.Add(image);
                }
                catch (Exception ex) when (ex is UriFormatException || ex is System.IO.FileNotFoundException)
                {
                    //Console.WriteLine($"Failed to load image: {ex.Message}");
                    var placeholder = new Image
                    {
                        Source = new BitmapImage(new Uri("C:\\Documents\\Labs\\код — копия\\Exchange_appl\\placeholder-image.jpg")),
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    itemPanel.Children.Add(placeholder);
                }
            }

            // Додаємо автора (номер телефону)
            var authorTextBlock = new TextBlock { Text = $"Автор: {item.AuthorPhone}", FontSize = 16 };
            itemPanel.Children.Add(authorTextBlock);

            return itemPanel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Вибір фото
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // Відображення прев'ю фото
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                ThumbnailImage.Source = bitmap;
                ThumbnailImage.Visibility = Visibility.Visible;

                // Збереження шляху до фото у приховане поле
                imageTextBox.Text = openFileDialog.FileName;
            }
        }
      /*  private void ItemClick(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as Button;
            if (button == null) return;

            // Get the associated Item from the DataContext
            var item = button.DataContext as Models.Item;
            if (item == null) return;

            // Use the item as needed
            var itemForm = new Views.ItemForm(item); // Pass the item to the new form (assuming the form accepts it)
            this.Hide(); // Hide the current form
            itemForm.Show(); // Show the new form
        }*/

        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter is Models.Item selectedItem)
            {
                // Відкриваємо нове вікно для редагування, передаємо посилання на MainForm
                var detailsWindow = new ItemForm(selectedItem, this);               
                detailsWindow.ShowDialog(); // Відкриваємо модально
            }
        }
        private void OffersToMeListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OffersToMeListView.SelectedItem is Models.Exchangeoffer selectedOffer)
            {
                if (selectedOffer.ItemRequested == null || selectedOffer.ItemRequested.Id == 0)
                {
                    MessageBox.Show("Запитуваний товар не вказано або має некоректний ідентифікатор.");
                    return;
                }

                // Ищем товар с указанным ID в базе данных
                var requestedItem = dbManager.GetItemById(selectedOffer.ItemRequested.Id);

                if (requestedItem == null)
                {
                    MessageBox.Show("Товар не знайдено.");
                    return;
                }

                // Открываем окно с найденным товаром
                var itemForm = new ItemForm(requestedItem, this);
                itemForm.ShowDialog();
            }
        }

        private void MyOffersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyOffersListView.SelectedItem is Models.Exchangeoffer selectedOffer)
            {
                if (selectedOffer.ItemOffered == null || selectedOffer.ItemOffered.Id == 0)
                {
                    MessageBox.Show("Товар, який ви пропонуєте, не вказано або має некоректний ідентифікатор.");
                    return;
                }

                // Ищем товар с указанным ID в базе данных
                var offeredItem = dbManager.GetItemById(selectedOffer.ItemOffered.Id);

                if (offeredItem == null)
                {
                    MessageBox.Show("Товар не знайдено.");
                    return;
                }

                // Открываем окно с найденным товаром
                var itemForm = new ItemForm(offeredItem, this);
                itemForm.ShowDialog();
            }
        }


        public void UpdateFilteredItems()
        {
            FilteredItems.Clear(); // Clear the existing items.

            if (string.IsNullOrEmpty(SelectedCategory) || SelectedCategory == "Всі")
            {
                foreach (var item in AllItems)
                {
                    FilteredItems.Add(item); // Add items back to the collection.
                }
            }
            else
            {
                foreach (var item in AllItems)
                {
                    if (item.Category == SelectedCategory)
                    {
                        FilteredItems.Add(item); // Add only the filtered items.
                    }
                }
            }
        }


        public void UpdateUserItems()
        {
            if (!DatabaseManager.CurrentUserId.HasValue)
            {
                MessageBox.Show("Будь ласка, увійдіть до системи, щоб переглядати ваші речі.");
                return;
            }

            // Очищаем текущий список
            UserItems.Clear();

            // Загружаем новые данные из базы
            var updatedItems = dbManager.GetAllUserItems(DatabaseManager.CurrentUserId.Value);

            // Добавляем обновленные данные в коллекцию
            foreach (var item in updatedItems)
            {
                UserItems.Add(item);
            }
            OnPropertyChanged(nameof(UserItems));
        }


        public void UpdateExchangeOffers()
        {
            if (!DatabaseManager.CurrentUserId.HasValue)
            {
                MessageBox.Show("Будь ласка, увійдіть до системи, щоб переглядати ваші запити на обмін.");
                return;
            }

            // Очищаем текущий список запросов
            OffersToMe.Clear();

            // Загружаем новые данные из базы
            var updatedOffers = dbManager.GetOffersToMe(DatabaseManager.CurrentUserId.Value);

            // Добавляем обновленные данные в коллекцию
            foreach (var offer in updatedOffers)
            {
                OffersToMe.Add(offer);
            }
        }
        public void UpdateMyOffers()
        {
            if (!DatabaseManager.CurrentUserId.HasValue)
            {
                MessageBox.Show("Будь ласка, увійдіть до системи, щоб переглядати ваші запити на обмін.");
                return;
            }

            // Очищаем текущий список запросов
            MyOffers.Clear();

            // Загружаем новые данные из базы
            var updatedOffers = dbManager.GetMyOffers(DatabaseManager.CurrentUserId.Value);

            // Добавляем обновленные данные в коллекцию
            foreach (var offer in updatedOffers)
            {
                MyOffers.Add(offer);
            }
        }

        private void AcceptOffer_Click(object sender, RoutedEventArgs e)
        {
            if (OffersToMeListView.SelectedItem is Exchangeoffer selectedOffer)
            {
                try
                {
                    selectedOffer.SetStatus("Accepted");
                    MessageBox.Show("Пропозицію прийнято!");
                    // Обновляем базу данных
                    UpdateExchangeOffers();
                    UpdateMyOffers();
                    DatabaseManager.UpdateExchangeOffer(selectedOffer);
                    OffersToMeListView.Items.Refresh();
                    MyOffersListView.Items.Refresh();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка прийняття пропозиції: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть пропозицію.");
            }
        }

        private void RejectOffer_Click(object sender, RoutedEventArgs e)
        {
            if (OffersToMeListView.SelectedItem is Exchangeoffer selectedOffer)
            {
                try
                {
                    selectedOffer.SetStatus("Rejected");
                    MessageBox.Show("Пропозицію відхилено!");
                    // Обновляем базу данных
                    DatabaseManager.UpdateExchangeOffer(selectedOffer);
                    UpdateMyOffers();
                    UpdateExchangeOffers();
                    OffersToMeListView.Items.Refresh();
                    MyOffersListView.Items.Refresh();
                    FilteredItems = new ObservableCollection<Models.Item>(FilteredItems);

                    UserItems = new ObservableCollection<Models.Item>(UserItems);

                    OnPropertyChanged(nameof(UserItems));

                    OnPropertyChanged(nameof(FilteredItems)); 



                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка відхилення пропозиції: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть пропозицію.");
            }
        }

        private void DeleteOffer_Click(object sender, RoutedEventArgs e)
        {
            if (MyOffersListView.SelectedItem is Exchangeoffer selectedOffer)
            {
                try
                {
                    // Удаляем предложение из базы данных
                    DatabaseManager.DeleteExchangeOffer(selectedOffer.Id);

                    // Удаляем предложение из ObservableCollection
                    MyOffers.Remove(selectedOffer);

                    MessageBox.Show("Пропозицію видалено!");

                    // Обновляем интерфейс (можно попытаться сделать это вручную)
                    MyOffersListView.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення пропозиції: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть пропозицію.");
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!DatabaseManager.CurrentUserId.HasValue)
            {
                MessageBox.Show("Помилка: користувач не авторизований.");
                return;
            }

            // Отримуємо дані з полів
            string itemName = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            string? category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string? exchangeCategory = (ExchangeCategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string imageFilePath = imageTextBox.Text;

            // Перевірка обов'язкових полів
            if (string.IsNullOrWhiteSpace(itemName))
            {
                MessageBox.Show("Помилка: поле 'Назва' не може бути порожнім.");
                return;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Помилка: поле 'Опис' не може бути порожнім.");
                return;
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("Помилка: виберіть категорію.");
                return;
            }
            if (string.IsNullOrWhiteSpace(exchangeCategory))
            {
                MessageBox.Show("Помилка: виберіть категорію для обміну.");
                return;
            }
            if (string.IsNullOrWhiteSpace(imageFilePath))
            {
                MessageBox.Show("Помилка: виберіть зображення.");
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(dbManager.ConnectionString))//змінив не склайт тут і далі
                {
                    conn.Open();

                    var command = new MySqlCommand(
                        "INSERT INTO Items (user_id, item_name, description, category, exchange_category, image) VALUES (@UserId, @ItemName, @Description, @Category, @ExchangeCategory, @Image)", conn);

                    command.Parameters.AddWithValue("@UserId", DatabaseManager.CurrentUserId);
                    command.Parameters.AddWithValue("@ItemName", itemName);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@ExchangeCategory", exchangeCategory);
                    command.Parameters.AddWithValue("@Image", imageFilePath);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Річ успішно додана.");

                    // Очищення полів після додавання
                    TitleTextBox.Clear();
                    DescriptionTextBox.Clear();
                    CategoryComboBox.SelectedIndex = -1;
                    ExchangeCategoryComboBox.SelectedIndex = -1;
                    ThumbnailImage.Source = null;
                    ThumbnailImage.Visibility = Visibility.Collapsed;
                    imageTextBox.Clear();

                    // Оновлюємо списки
                    LoadAllItems();
                    UpdateUserItems();
                    LoadMyItems();

                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySql помилка: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Загальна помилка: {ex.Message}");
            }
        }


    }
}