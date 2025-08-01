using Exchange_appl;
using System;
using System.Windows;


namespace Exchange_appl.Views
{
    public partial class WelcomeForm : Window
    {
        public WelcomeForm()
        {
            InitializeComponent();
        }

        private void entry_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно для входа
            LoginForm loginForm = new LoginForm();
            loginForm.Show(); // Показываем новую форму
        }

        private void registration_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно для регистрации
            RegistrationForm registrationForm = new RegistrationForm();
            registrationForm.Show(); // Показываем новую форму
        }

        private void admin_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно для администратора
            AdminForm adminForm = new AdminForm();
            adminForm.Show(); // Показываем новую форму
        }
    }
}
