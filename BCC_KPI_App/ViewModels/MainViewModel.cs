using System.Windows.Input;
using BCC_KPI_App.Helpers;

namespace BCC_KPI_App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentPage;

        public MainViewModel()
        {
            // Показываем дашборд сразу при запуске
            _currentPage = new DashboardViewModel();
        }

        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public ICommand ShowDashboardCommand => new RelayCommand(p =>
        {
            CurrentPage = new DashboardViewModel();
        });

        public ICommand ShowReportsCommand => new RelayCommand(p =>
        {
            CurrentPage = new ReportsViewModel();
        });

        public ICommand ShowAdminCommand => new RelayCommand(p =>
        {
            CurrentPage = new AdminViewModel();
        });
    }
}