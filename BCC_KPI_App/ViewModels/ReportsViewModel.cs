using BCC_KPI_App.Helpers;
using BCC_KPI_App.Models;
using BCC_KPI_App.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data;

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

                var units = _context.Units.ToList();
                foreach (var unit in units)
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

                if (ReportData.Count == 0)
                {
                    System.Windows.MessageBox.Show("За выбранный период данных не найдено.", "Информация",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка формирования отчета: {ex.Message}", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExportToExcel()
        {
            if (ReportData.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта. Сначала сформируйте отчет.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dt = new DataTable("KPI_Report");
            dt.Columns.Add("Подразделение", typeof(string));
            dt.Columns.Add("План (руб)", typeof(decimal));
            dt.Columns.Add("Факт (руб)", typeof(decimal));
            dt.Columns.Add("Выполнение (%)", typeof(string));

            foreach (var item in ReportData)
            {
                dt.Rows.Add(item.Подразделение, item.План, item.Факт, item.Выполнение);
            }

            Services.ExcelExportService.ExportToExcel(dt, $"KPI_Report_{DateTime.Now:yyyyMMdd}",
                null, null, "Отчет по KPI холдинга БЦК");
        }


        private void ExportToPdf()
        {
            System.Windows.MessageBox.Show("Экспорт в PDF будет доступен в следующей версии.", "В разработке",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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