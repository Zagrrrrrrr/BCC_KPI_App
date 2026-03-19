using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BCC_KPI_App.Models;
using BCC_KPI_App.Helpers;

namespace BCC_KPI_App.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly SalesContext _context;
        private ObservableCollection<Unit> _units;
        private Unit _selectedUnit;
        private string _searchText;

        public AdminViewModel()
        {
            _context = new SalesContext();
            _units = new ObservableCollection<Unit>();

            LoadUnits();

            SearchCommand = new RelayCommand(p => SearchUnits());
            AddUnitCommand = new RelayCommand(p => AddUnit());
            EditCommand = new RelayCommand(p => EditUnit(p as Unit));
            DeleteCommand = new RelayCommand(p => DeleteUnit(p as Unit));
        }

        public ObservableCollection<Unit> Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(nameof(Units)); }
        }

        public Unit SelectedUnit
        {
            get => _selectedUnit;
            set { _selectedUnit = value; OnPropertyChanged(nameof(SelectedUnit)); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddUnitCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private void LoadUnits()
        {
            try
            {
                Units = new ObservableCollection<Unit>(_context.Units.ToList());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void SearchUnits()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadUnits();
                }
                else
                {
                    var filtered = _context.Units
                        .Where(u => u.UnitName.Contains(SearchText)
                                  || u.City.Contains(SearchText)
                                  || u.UnitType.Contains(SearchText))
                        .ToList();

                    Units = new ObservableCollection<Unit>(filtered);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка поиска: {ex.Message}");
            }
        }

        private void AddUnit()
        {
            var dialog = new UnitDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _context.Units.Add(dialog.Unit);
                    _context.SaveChanges();
                    LoadUnits();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Ошибка добавления: {ex.Message}");
                }
            }
        }

        private void EditUnit(Unit unit)
        {
            if (unit == null) return;

            var dialog = new UnitDialog(unit);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _context.SaveChanges();
                    LoadUnits();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Ошибка обновления: {ex.Message}");
                }
            }
        }

        private void DeleteUnit(Unit unit)
        {
            if (unit == null) return;

            var result = System.Windows.MessageBox.Show(
                $"Удалить подразделение '{unit.UnitName}'?",
                "Подтверждение",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    _context.Units.Remove(unit);
                    _context.SaveChanges();
                    LoadUnits();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }
    }
}