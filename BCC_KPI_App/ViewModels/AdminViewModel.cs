using BCC_KPI_App.Helpers;
using BCC_KPI_App.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadUnits();
            }
            else
            {
                var filtered = _context.Units
                    .Where(u => u.UnitName.Contains(SearchText)
                              || u.City.Contains(SearchText))
                    .ToList();
                Units = new ObservableCollection<Unit>(filtered);
            }
        }

        private void AddUnit()
        {
            var dialog = new UnitDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var newUnit = dialog.Unit;

                    // Получаем ID из справочников
                    var unitType = _context.UnitTypes.FirstOrDefault(t => t.TypeName == newUnit.UnitType);
                    var unitStatus = _context.UnitStatuses.FirstOrDefault(s => s.StatusName == newUnit.Status);
                    var holding = _context.Holdings.FirstOrDefault();

                    if (unitType == null || unitStatus == null || holding == null)
                    {
                        MessageBox.Show("Не найдены справочные данные! Добавьте типы, статусы и холдинг в БД.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    newUnit.UnitTypeID = unitType.TypeID;
                    newUnit.StatusID = unitStatus.StatusID;
                    newUnit.HoldingID = holding.HoldingID;

                    _context.Units.Add(newUnit);
                    _context.SaveChanges();
                    LoadUnits();
                    MessageBox.Show("Подразделение добавлено!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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
                    // Получаем обновленные данные из диалога
                    var updatedUnit = dialog.Unit;

                    // Находим существующую запись в контексте
                    var existingUnit = _context.Units.Find(unit.UnitId);
                    if (existingUnit != null)
                    {
                        existingUnit.UnitName = updatedUnit.UnitName;
                        existingUnit.City = updatedUnit.City;
                        existingUnit.UnitType = updatedUnit.UnitType;
                        existingUnit.Status = updatedUnit.Status;

                        // Получаем ID из справочников
                        var unitType = _context.UnitTypes.FirstOrDefault(t => t.TypeName == updatedUnit.UnitType);
                        var unitStatus = _context.UnitStatuses.FirstOrDefault(s => s.StatusName == updatedUnit.Status);
                        var holding = _context.Holdings.FirstOrDefault();

                        if (unitType != null) existingUnit.UnitTypeID = unitType.TypeID;
                        if (unitStatus != null) existingUnit.StatusID = unitStatus.StatusID;
                        if (holding != null) existingUnit.HoldingID = holding.HoldingID;

                        _context.SaveChanges();
                        LoadUnits();
                        MessageBox.Show("Подразделение обновлено!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteUnit(Unit unit)
        {
            if (unit == null) return;

            var result = MessageBox.Show(
                $"Удалить подразделение '{unit.UnitName}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var existingUnit = _context.Units.Find(unit.UnitId);
                    if (existingUnit != null)
                    {
                        _context.Units.Remove(existingUnit);
                        _context.SaveChanges();
                        LoadUnits();
                        MessageBox.Show("Подразделение удалено!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}