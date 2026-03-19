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

        public AdminViewModel()
        {
            _context = new SalesContext();
            _units = new ObservableCollection<Unit>();

            LoadUnits();
        }

        public ObservableCollection<Unit> Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(); }
        }

        public Unit SelectedUnit
        {
            get => _selectedUnit;
            set { _selectedUnit = value; OnPropertyChanged(); }
        }

        private void LoadUnits()
        {
            _units.Clear();
            foreach (var u in _context.Units.ToList())
            {
                _units.Add(u);
            }
        }
    }
}