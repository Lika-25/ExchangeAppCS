using Exchange_appl;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Exchange_appl.Views
{
    public partial class LoginForm : Window
    {
        //private DatabaseManager dbManager;
        private DatabaseManager dbManager;
        public LoginForm()
        {
            InitializeComponent();
            //dbManager = new DatabaseManager(); // Инициализация менеджера базы данных
            dbManager = new DatabaseManager();
        }

        // Обработчик для кнопки видимости пароля
        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Text = PasswordBox.Password;
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Password = PasswordTextBox.Text;
            }
        }

        // Обработчик для кнопки входа
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = PhoneNumberTextBox.Text;
            string password = PasswordBox.Password;

            // Логика для проверки данных
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите номер телефона и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Проверка аутентификации
                if (dbManager.AuthenticateUser(phoneNumber, password))
                {
                    MessageBox.Show("Вход выполнен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Переход на главную форму
                    var mainForm = new Views.MainForm(); // Замена на реальную форму
                    mainForm.Show(); // Открытие новой формы
                    this.Close(); // Скрытие текущей формы

                }
                else
                {
                    MessageBox.Show("Неверный номер телефона или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordForm forgotPasswordWindow = new ForgotPasswordForm();
            forgotPasswordWindow.Show();  // Показати вікно
        }

    }
}
