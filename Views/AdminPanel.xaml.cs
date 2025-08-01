using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Exchange_appl.Views
{
    public partial class AdminPanel : Window
    {
        private readonly DatabaseManager dbManager;

        public AdminPanel()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
            LoadData(); // Завантаження даних при ініціалізації форми
        }

        private void LoadData()
        {
            LoadUsers(); // Завантаження користувачів
            LoadItems(); // Завантаження предметів
        }

        private void LoadUsers()
        {
            var users = DatabaseManager.LoadUsersFromXml("users.xml");
            if (users.Count == 0)
            {
                users = dbManager.GetAllUsers(); // Якщо XML порожній, завантажуємо з бази даних
            }

            if (users != null && users.Any())
            {
                foreach (var user in users)
                {
                    user.Username ??= "Невідомий користувач";
                    user.Password ??= "Невідомо";
                }

                dataGridViewUsers.ItemsSource = users; // Відображення користувачів у DataGrid
            }
            else
            {
                MessageBox.Show("Не вдалося завантажити користувачів.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                dataGridViewUsers.ItemsSource = new List<User>(); // Заповнюємо порожній список
            }
        }

        private void LoadItems()
        {
            var items = dbManager.GetAllItems().ToList();
            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    item.ItemName ??= "Невідомий предмет";
                    item.Description ??= "Опис відсутній";
                    item.Category ??= "Невідома категорія";
                    item.ExchangeCategory ??= "Не визначено";
                    item.Image ??= "Зображення відсутнє";
                    item.ExchangeofferItemOffereds.Clear();
                    item.ExchangeofferItemRequesteds.Clear();
                }

                dataGridViewItems.ItemsSource = items;
            }
            else
            {
                MessageBox.Show("Не вдалося завантажити предмети.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                dataGridViewItems.ItemsSource = new List<Models.Item>(); // Заповнюємо порожній список
            }
        }

        private void dataGridViewUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridViewUsers.SelectedItem is User selectedUser)
            {
                LoadUserItems(selectedUser.Id);
            }
        }

        private void LoadUserItems(int userId)
        {
            var items = dbManager.GetAllUserItems(userId);
            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    item.ItemName ??= "Невідомий предмет";
                    item.Description ??= "Опис відсутній";
                    item.Category ??= "Невідома категорія";
                    item.ExchangeCategory ??= "Не визначено";
                    item.Image ??= "Зображення відсутнє";
                    item.ExchangeofferItemOffereds.Clear();
                    item.ExchangeofferItemRequesteds.Clear();
                }

                dataGridViewItems.ItemsSource = items;
            }
            else
            {
                dataGridViewItems.ItemsSource = new List<Item>(); // Якщо предметів немає, заповнюємо порожній список
            }
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка, чи вибраний користувач
            if (dataGridViewUsers.SelectedItem is User currentUser)
            {
                var result = MessageBox.Show("Ви впевнені, що хочете видалити цього користувача?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    dbManager.DeleteUser(currentUser.Id);
                    var userItems = dbManager.GetAllUserItems(currentUser.Id);
                    foreach (var item in userItems)
                    {
                        dbManager.DeleteItem(item.Id);
                    }

                    dbManager.SaveUsersToXml("users.xml");
                    LoadUsers();
                    dataGridViewItems.ItemsSource = new List<Item>();
                }
            }
            // Перевірка, чи вибраний предмет
            else if (dataGridViewItems.SelectedItem is Item selectedItem)
            {
                // Перевірка, чи вибраний користувач
                if (dataGridViewUsers.SelectedItem is User selectedUser)
                {
                    var result = MessageBox.Show($"Ви впевнені, що хочете видалити цей предмет користувача {selectedUser.Username}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        dbManager.DeleteItem(selectedItem.Id);
                        LoadUserItems(selectedUser.Id); // Оновлюємо список речей користувача після видалення
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, виберіть користувача перед видаленням предмета.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть користувача або предмет для видалення.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AdminPanel_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dbManager.SaveUsersToXml("users.xml");
        }
    }
}
