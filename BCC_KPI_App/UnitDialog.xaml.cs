using BCC_KPI_App.Models;
using System.Windows;
using System.Windows.Controls;

namespace BCC_KPI_App
{
    public partial class UnitDialog : Window
    {
        public Unit Unit { get; set; }

        public UnitDialog(Unit unit = null)
        {
            InitializeComponent();

            if (unit == null)
            {
                Unit = new Unit();
                Unit.UnitType = "Завод";
                Unit.Status = "Активен";
                Unit.City = "";
                Unit.UnitName = "";
            }
            else
            {
                Unit = unit;

                // Заполняем поля из существующей записи
                txtName.Text = unit.UnitName;
                txtCity.Text = unit.City;

                // Выбираем тип
                if (unit.UnitType == "Завод")
                    cmbType.SelectedIndex = 0;
                else if (unit.UnitType == "Торговый дом")
                    cmbType.SelectedIndex = 1;

                // Выбираем статус
                if (unit.Status == "Активен")
                    cmbStatus.SelectedIndex = 0;
                else if (unit.Status == "Реорганизация")
                    cmbStatus.SelectedIndex = 1;
                else if (unit.Status == "На консервации")
                    cmbStatus.SelectedIndex = 2;
            }

            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Заполняем Unit из полей формы
            Unit.UnitName = txtName.Text.Trim();
            Unit.City = txtCity.Text.Trim();

            // Получаем выбранный тип
            var selectedType = cmbType.SelectedItem as ComboBoxItem;
            if (selectedType != null)
                Unit.UnitType = selectedType.Content.ToString();

            // Получаем выбранный статус
            var selectedStatus = cmbStatus.SelectedItem as ComboBoxItem;
            if (selectedStatus != null)
                Unit.Status = selectedStatus.Content.ToString();

            if (string.IsNullOrWhiteSpace(Unit.UnitName))
            {
                MessageBox.Show("Введите название подразделения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Unit.City))
            {
                MessageBox.Show("Введите город!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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