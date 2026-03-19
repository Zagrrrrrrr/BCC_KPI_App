using System.Windows;
using BCC_KPI_App.Models;

namespace BCC_KPI_App
{
    public partial class UnitDialog : Window
    {
        public Unit Unit { get; set; }

        public UnitDialog(Unit unit = null)
        {
            InitializeComponent();

            if (unit == null)
                Unit = new Unit();
            else
                Unit = unit;

            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}