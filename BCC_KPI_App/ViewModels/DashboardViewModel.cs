using System;
using System.Collections.ObjectModel;
using BCC_KPI_App.Models;

namespace BCC_KPI_App.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private ObservableCollection<ChartData> _chartData;

        public DashboardViewModel()
        {
            try
            {
                _chartData = new ObservableCollection<ChartData>();

                // Тестовые данные из твоей БД
                _chartData.Add(new ChartData { UnitName = "ОАО Кричевцементношифер", TargetValue = 500000, ActualValue = 350000 });
                _chartData.Add(new ChartData { UnitName = "Торговый дом БЦК - Минск", TargetValue = 300000, ActualValue = 280000 });
                _chartData.Add(new ChartData { UnitName = "ОАО Красносельскстройматериалы", TargetValue = 400000, ActualValue = 200000 });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        public ObservableCollection<ChartData> ChartData
        {
            get { return _chartData; }
            set
            {
                _chartData = value;
                OnPropertyChanged(nameof(ChartData));
            }
        }
    }
}