using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using BCC_KPI_App.Models;
using BCC_KPI_App.Helpers;

namespace BCC_KPI_App.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly SalesContext _context;
        private ObservableCollection<ChartData> _chartData;
        private decimal _totalTarget;
        private decimal _totalActual;
        private DateTime _startDate;
        private DateTime _endDate;
        private SeriesCollection _comparisonSeries;
        private string[] _unitNames;
        private Func<double, string> _formatter;
        private ObservableCollection<Unit> _units;
        private Unit _selectedUnit;

        public DashboardViewModel()
        {
            _context = new SalesContext();
            _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _endDate = DateTime.Now;
            _chartData = new ObservableCollection<ChartData>();
            _units = new ObservableCollection<Unit>();

            LoadUnits();
            LoadData();

            ApplyFilterCommand = new RelayCommand(p => LoadData());
            CompareCommand = new RelayCommand(p => CompareWithPreviousPeriod());
            ExportCommand = new RelayCommand(p => ExportData());
        }

        // Свойства
        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(nameof(StartDate)); }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(nameof(EndDate)); }
        }

        public ObservableCollection<Unit> Units
        {
            get => _units;
            set { _units = value; OnPropertyChanged(nameof(Units)); }
        }

        public Unit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                OnPropertyChanged(nameof(SelectedUnit));
                LoadData();
            }
        }

        public ObservableCollection<ChartData> ChartData
        {
            get => _chartData;
            set
            {
                _chartData = value;
                OnPropertyChanged(nameof(ChartData));
                CalculateTotals();
                UpdateCharts();
            }
        }

        public decimal TotalTarget
        {
            get => _totalTarget;
            set { _totalTarget = value; OnPropertyChanged(nameof(TotalTarget)); }
        }

        public decimal TotalActual
        {
            get => _totalActual;
            set { _totalActual = value; OnPropertyChanged(nameof(TotalActual)); }
        }

        public string TotalPercentage
        {
            get
            {
                if (TotalTarget > 0)
                    return $"{Math.Round((TotalActual / TotalTarget) * 100, 2)}%";
                return "0%";
            }
        }

        public int UnitsCount => ChartData?.Count ?? 0;

        // Для графиков
        public SeriesCollection ComparisonSeries
        {
            get => _comparisonSeries;
            set { _comparisonSeries = value; OnPropertyChanged(nameof(ComparisonSeries)); }
        }

        public string[] UnitNames
        {
            get => _unitNames;
            set { _unitNames = value; OnPropertyChanged(nameof(UnitNames)); }
        }

        public Func<double, string> Formatter
        {
            get => _formatter;
            set { _formatter = value; OnPropertyChanged(nameof(Formatter)); }
        }

        // Команды
        public ICommand ApplyFilterCommand { get; private set; }
        public ICommand CompareCommand { get; }
        public ICommand ExportCommand { get; }

        private void LoadUnits()
        {
            try
            {
                Units = new ObservableCollection<Unit>(_context.Units.ToList());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки подразделений: {ex.Message}");
            }
        }

        private void CalculateTotals()
        {
            TotalTarget = ChartData?.Sum(x => x.TargetValue) ?? 0;
            TotalActual = ChartData?.Sum(x => x.ActualValue) ?? 0;
        }

        private void LoadData()
        {
            try
            {
                var data = new ObservableCollection<ChartData>();

                var unitsQuery = _context.Units.AsQueryable();

                if (SelectedUnit != null)
                {
                    unitsQuery = unitsQuery.Where(u => u.UnitId == SelectedUnit.UnitId);
                }

                foreach (var unit in unitsQuery.ToList())
                {
                    var target = _context.KpiTargets
                        .Where(t => t.UnitId == unit.UnitId
                            && t.PeriodStart >= StartDate
                            && t.PeriodEnd <= EndDate)
                        .Sum(t => (decimal?)t.TargetValue) ?? 0;

                    var actual = _context.KpiActuals
                        .Where(a => a.UnitId == unit.UnitId
                            && a.SaleDate >= StartDate
                            && a.SaleDate <= EndDate)
                        .Sum(a => (decimal?)a.ActualValue) ?? 0;

                    data.Add(new ChartData
                    {
                        UnitName = unit.UnitName,
                        TargetValue = target,
                        ActualValue = actual
                    });
                }

                ChartData = data;

                // ЖЕСТКИЙ КОСТЫЛЬ - пересоздаем график
                UpdateCharts();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");

                ChartData = new ObservableCollection<ChartData>
        {
            new ChartData { UnitName = "Кричевцементношифер", TargetValue = 500000, ActualValue = 350000 },
            new ChartData { UnitName = "ТД БЦК - Минск", TargetValue = 300000, ActualValue = 280000 },
            new ChartData { UnitName = "Красносельскстройматериалы", TargetValue = 400000, ActualValue = 200000 }
        };
                UpdateCharts();
            }
        }

        private void UpdateCharts()
        {
            try
            {
                // Полностью убиваем старый график
                ComparisonSeries = null;
                UnitNames = null;
                OnPropertyChanged(nameof(ComparisonSeries));
                OnPropertyChanged(nameof(UnitNames));

                if (ChartData != null && ChartData.Count > 0)
                {
                    // Создаем новый график
                    var newSeries = new SeriesCollection();

                    newSeries.Add(new ColumnSeries
                    {
                        Title = "План",
                        Values = new ChartValues<decimal>(ChartData.Select(x => x.TargetValue)),
                        Fill = System.Windows.Media.Brushes.SteelBlue
                    });

                    newSeries.Add(new ColumnSeries
                    {
                        Title = "Факт",
                        Values = new ChartValues<decimal>(ChartData.Select(x => x.ActualValue)),
                        Fill = System.Windows.Media.Brushes.ForestGreen
                    });

                    ComparisonSeries = newSeries;
                    UnitNames = ChartData.Select(x => x.UnitName).ToArray();
                    Formatter = value => value.ToString("N0");
                }

                // Двойной вызов для верности
                OnPropertyChanged(nameof(ComparisonSeries));
                OnPropertyChanged(nameof(UnitNames));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка обновления графиков: {ex.Message}");
            }
        }

        private void CompareWithPreviousPeriod()
        {
            try
            {
                var daysDiff = (EndDate - StartDate).Days;
                var previousStart = StartDate.AddDays(-daysDiff);
                var previousEnd = StartDate.AddDays(-1);

                var message = "Сравнение с предыдущим периодом:\n\n";

                foreach (var current in ChartData)
                {
                    var unit = _context.Units.FirstOrDefault(u => u.UnitName == current.UnitName);
                    if (unit != null)
                    {
                        var prevActual = _context.KpiActuals
                            .Where(a => a.UnitId == unit.UnitId
                                && a.SaleDate >= previousStart
                                && a.SaleDate <= previousEnd)
                            .Sum(a => (decimal?)a.ActualValue) ?? 0;

                        var diff = current.ActualValue - prevActual;
                        var percent = prevActual > 0
                            ? Math.Round((current.ActualValue / prevActual - 1) * 100, 2)
                            : 0;

                        message += $"{current.UnitName}: {diff:N0} ({percent:+#;-#;0}%)\n";
                    }
                }

                System.Windows.MessageBox.Show(message, "Сравнение с прошлым периодом");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка сравнения: {ex.Message}");
            }
        }

        private void ExportData()
        {
            System.Windows.MessageBox.Show("Экспорт данных в разработке");
        }
    }
}