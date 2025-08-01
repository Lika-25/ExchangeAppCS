using Exchange_appl;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Exchange_appl.Views
{
    public partial class RegistrationForm : Window
    {
        public RegistrationForm()
        {
            InitializeComponent();

        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            // Перемикання видимості паролів
            if (PasswordBox1.Visibility == Visibility.Visible)
            {
                PasswordBox1.Visibility = Visibility.Collapsed;
                PasswordBox2.Visibility = Visibility.Collapsed;

                PasswordTextBox1.Visibility = Visibility.Visible;
                PasswordTextBox2.Visibility = Visibility.Visible;

                PasswordTextBox1.Text = PasswordBox1.Password;
                PasswordTextBox2.Text = PasswordBox2.Password;
            }
            else
            {
                PasswordBox1.Visibility = Visibility.Visible;
                PasswordBox2.Visibility = Visibility.Visible;

                PasswordTextBox1.Visibility = Visibility.Collapsed;
                PasswordTextBox2.Visibility = Visibility.Collapsed;

                PasswordBox1.Password = PasswordTextBox1.Text;
                PasswordBox2.Password = PasswordTextBox2.Text;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = PhoneNumberTextBox.Text; // Номер телефону
            string password1 = PasswordBox1.Password; // Перший пароль
            string password2 = PasswordBox2.Password; // Другий пароль

            // Перевірка на збіг паролів
            if (password1 != password2)
            {
                MessageBox.Show("Паролі не збігаються!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dbManager = new DatabaseManager();
            try
            {
                dbManager.AddUser(username, password1); // Запис користувача в БД
                MessageBox.Show("Реєстрація успішна!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                // Получение ID пользователя для хранения в глобальной переменной
                DatabaseManager.CurrentUserId = dbManager.GetUserIdByUsername(username);

                // Збереження користувачів в XML
                dbManager.SaveUsersToXml("users.xml");

                // Перехід на головну форму або іншу
                var mainForm = new MainForm();
                this.Hide();
                mainForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
