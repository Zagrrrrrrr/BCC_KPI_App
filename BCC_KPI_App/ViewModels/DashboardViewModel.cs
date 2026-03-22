using BCC_KPI_App.Helpers;
using BCC_KPI_App.Models;
using BCC_KPI_App.Services;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data;

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
        private SeriesCollection _dynamicsSeries;
        private string[] _unitNames;
        private string[] _monthLabels;
        private Func<double, string> _formatter;
        private ObservableCollection<Unit> _units;
        private Unit _selectedUnit;
        public FrameworkElement ChartComparison { get; set; }
        public FrameworkElement ChartDynamics { get; set; }

        public DashboardViewModel()
        {
            _context = new SalesContext();
            _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _endDate = DateTime.Now;
            _chartData = new ObservableCollection<ChartData>();
            _units = new ObservableCollection<Unit>();

            LoadUnits();
            LoadData();
            LoadDynamicsData();

            ApplyFilterCommand = new RelayCommand(p => { LoadData(); LoadDynamicsData(); });
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
                LoadDynamicsData();
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

        public SeriesCollection DynamicsSeries
        {
            get => _dynamicsSeries;
            set { _dynamicsSeries = value; OnPropertyChanged(nameof(DynamicsSeries)); }
        }

        public string[] UnitNames
        {
            get => _unitNames;
            set { _unitNames = value; OnPropertyChanged(nameof(UnitNames)); }
        }

        public string[] MonthLabels
        {
            get => _monthLabels;
            set { _monthLabels = value; OnPropertyChanged(nameof(MonthLabels)); }
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
                var unitsQuery = SelectedUnit != null
                    ? _context.Units.Where(u => u.UnitId == SelectedUnit.UnitId).ToList()
                    : _context.Units.ToList();

                foreach (var unit in unitsQuery)
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
                UpdateCharts();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadDynamicsData()
        {
            try
            {
                var months = new System.Collections.Generic.List<string>();
                var actualValues = new System.Collections.Generic.List<decimal>();

                // Последние 6 месяцев
                for (int i = 5; i >= 0; i--)
                {
                    var month = DateTime.Now.AddMonths(-i);
                    months.Add(month.ToString("MMM yyyy"));

                    var actual = _context.KpiActuals
                        .Where(a => a.SaleDate.Year == month.Year && a.SaleDate.Month == month.Month)
                        .Sum(a => (decimal?)a.ActualValue) ?? 0;
                    actualValues.Add(actual);
                }

                MonthLabels = months.ToArray();
                DynamicsSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Факт",
                        Values = new ChartValues<decimal>(actualValues),
                        PointGeometry = null,
                        StrokeThickness = 3,
                        Fill = System.Windows.Media.Brushes.Transparent
                    }
                };
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки динамики: {ex.Message}");
            }
        }

        private void UpdateCharts()
        {
            try
            {
                ComparisonSeries = null;
                UnitNames = null;
                OnPropertyChanged(nameof(ComparisonSeries));
                OnPropertyChanged(nameof(UnitNames));

                if (ChartData != null && ChartData.Count > 0)
                {
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

                var message = "📊 Сравнение с предыдущим периодом:\n\n";

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

                        message += $"🏭 {current.UnitName}:\n";
                        message += $"   Было: {prevActual:N0} руб.\n";
                        message += $"   Стало: {current.ActualValue:N0} руб.\n";
                        message += $"   Динамика: {diff:N0} руб. ({percent:+#;-#;0}%)\n\n";
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
            try
            {
                var dt = new DataTable("KPI_Report");
                dt.Columns.Add("Подразделение", typeof(string));
                dt.Columns.Add("План (руб)", typeof(decimal));
                dt.Columns.Add("Факт (руб)", typeof(decimal));
                dt.Columns.Add("Выполнение (%)", typeof(decimal));

                foreach (var item in ChartData)
                {
                    dt.Rows.Add(item.UnitName, item.TargetValue, item.ActualValue, item.CompletionPercentage);
                }

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Services.ExcelExportService.ExportToExcel(dt, $"KPI_Report_{DateTime.Now:yyyyMMdd}",
                    ChartComparison, ChartDynamics, "Отчет по KPI холдинга БЦК");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}