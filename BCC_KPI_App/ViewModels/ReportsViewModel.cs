using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BCC_KPI_App.Models;
using BCC_KPI_App.Helpers;

namespace BCC_KPI_App.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        private readonly SalesContext _context;
        private DateTime _startDate;
        private DateTime _endDate;
        private ObservableCollection<ReportItem> _reportData;

        public ReportsViewModel()
        {
            _context = new SalesContext();
            _startDate = new DateTime(DateTime.Now.Year, 1, 1);
            _endDate = DateTime.Now;
            _reportData = new ObservableCollection<ReportItem>();

            GenerateReportCommand = new RelayCommand(p => GenerateReport());
            ExportToExcelCommand = new RelayCommand(p => ExportToExcel());
            ExportToPdfCommand = new RelayCommand(p => ExportToPdf());
        }

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

        public ObservableCollection<ReportItem> ReportData
        {
            get => _reportData;
            set { _reportData = value; OnPropertyChanged(nameof(ReportData)); }
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ExportToPdfCommand { get; }

        private void GenerateReport()
        {
            try
            {
                ReportData.Clear();

                foreach (var unit in _context.Units.ToList())
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

                    var completion = target > 0
                        ? Math.Round((actual / target) * 100, 2)
                        : 0;

                    ReportData.Add(new ReportItem
                    {
                        Подразделение = unit.UnitName,
                        План = target,
                        Факт = actual,
                        Выполнение = $"{completion}%"
                    });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка формирования отчета: {ex.Message}");
            }
        }

        private void ExportToExcel()
        {
            // TODO: Реализовать экспорт в Excel
            System.Windows.MessageBox.Show("Экспорт в Excel будет доступен в следующей версии");
        }

        private void ExportToPdf()
        {
            // TODO: Реализовать экспорт в PDF
            System.Windows.MessageBox.Show("Экспорт в PDF будет доступен в следующей версии");
        }
    }

    public class ReportItem
    {
        public string Подразделение { get; set; }
        public decimal План { get; set; }
        public decimal Факт { get; set; }
        public string Выполнение { get; set; }
    }
}