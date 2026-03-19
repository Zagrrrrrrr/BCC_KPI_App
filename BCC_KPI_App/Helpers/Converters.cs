using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using LiveCharts;
using BCC_KPI_App.Models;

namespace BCC_KPI_App.Helpers
{
    public class UnitNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<ChartData> data && data != null)
            {
                return data.Select(d => d.UnitName).ToArray();
            }
            return new string[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TargetValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<ChartData> data && data != null)
            {
                return new ChartValues<decimal>(data.Select(d => d.TargetValue));
            }
            return new ChartValues<decimal>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ActualValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<ChartData> data && data != null)
            {
                return new ChartValues<decimal>(data.Select(d => d.ActualValue));
            }
            return new ChartValues<decimal>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProgressColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal percentage)
            {
                if (percentage >= 100)
                    return new SolidColorBrush(Color.FromRgb(39, 174, 96));
                if (percentage >= 70)
                    return new SolidColorBrush(Color.FromRgb(243, 156, 18));
                return new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }
            return new SolidColorBrush(Color.FromRgb(127, 140, 141));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}