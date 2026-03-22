using System.Windows;

namespace BCC_KPI_App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DashboardPage());
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new DashboardPage());
        private void Reports_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ReportsPage());
        private void Admin_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AdminPage());
        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}