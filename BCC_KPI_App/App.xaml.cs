using System.Windows;

namespace BCC_KPI_App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var main = new MainWindow();
            main.Show();
        }
    }
}