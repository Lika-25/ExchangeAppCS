using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Exchange_appl.Models; // Подключение модели User
using Twilio;               // Для работы с Twilio API

namespace Exchange_appl.Views
{
    public partial class ForgotPasswordForm : Window
    {
        private readonly SmsService _smsService;
        private readonly Dictionary<string, string> _otpStorage;

        public ForgotPasswordForm()
        {
            InitializeComponent();
            _smsService = new SmsService();
            _otpStorage = new Dictionary<string, string>();
        }

        // Обработчик для кнопки "Надіслати код"
        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Будь ласка, введіть номер телефону.");
                return;
            }

            var db = new DatabaseManager();
            var users = db.GetUsers(); // Получаем список всех пользователей

            var userExists = users.Any(u => u.Username == username);

            if (!userExists)
            {
                MessageBox.Show("Номер телефону не знайдено.");
                return;
            }

            var otpCode = GenerateOneTimeCode();
            _otpStorage[username] = otpCode;

            try
            {
                _smsService.SendSms(username, $"Ваш одноразовий код: {otpCode}");
                MessageBox.Show("Код відправлено на ваш номер телефону.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при відправці SMS: {ex.Message}");
            }
        }

        private void ConfirmCodeButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var enteredCode = OneTimeCodeTextBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(enteredCode))
            {
                MessageBox.Show("Будь ласка, введіть номер телефону та код.");
                return;
            }

            if (_otpStorage.ContainsKey(username) && _otpStorage[username] == enteredCode)
            {
                // Отображаем форму для ввода нового пароля
                NewPasswordPanel.Visibility = Visibility.Visible;
                MessageBox.Show("Код підтверджено. Придумайте новий пароль.");
            }
            else
            {
                MessageBox.Show("Невірний одноразовий код.");
            }
        }

        private void SaveNewPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var newPassword = NewPasswordTextBox.Password;
            var confirmPassword = ConfirmPasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Будь ласка, введіть новий пароль та підтвердження паролю.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Паролі не співпадають.");
                return;
            }

            var db = new DatabaseManager();

            // Хешируем новый пароль
            var hashedPassword = DatabaseManager.HashPassword(newPassword);  // Статический вызов метода

            // Обновление пароля в базе данных
            var users = db.GetUsers();
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                // Преобразуем объект user в Models.User (если нужно)
                var userModel = new Exchange_appl.Models.User
                {
                    Username = user.Username,
                    Password = hashedPassword // Сохраняем хешированный пароль
                };

                db.UpdateUser(userModel); // Обновляем пароль пользователя в базе данных
                MessageBox.Show("Пароль успішно змінено!");

                this.Close(); // Закрываем форму восстановления пароля
            }
            else
            {
                MessageBox.Show("Користувача не знайдено.");
            }
        }





        // Метод для генерации одноразового кода
        private string GenerateOneTimeCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-значный код
        }
    }
}
