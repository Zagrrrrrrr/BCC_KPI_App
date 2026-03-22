using System.Windows;

namespace BCC_KPI_App
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите логин и пароль!";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            // Пока без БД — простая проверка
            if (username == "admin" && password == "admin123")
            {
                DialogResult = true;
                Close();
            }
            else
            {
                txtError.Text = "Неверный логин или пароль!";
                txtError.Visibility = Visibility.Visible;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}