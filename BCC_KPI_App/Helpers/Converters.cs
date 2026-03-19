using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BCC_KPI_App.Helpers
{
    public class PercentToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal percentage)
            {
                if (percentage >= 100)
                    return new SolidColorBrush(Color.FromRgb(46, 204, 113));
                if (percentage >= 70)
                    return new SolidColorBrush(Color.FromRgb(241, 196, 15));
                if (percentage >= 50)
                    return new SolidColorBrush(Color.FromRgb(230, 126, 34));
                return new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }
            return new SolidColorBrush(Color.FromRgb(52, 73, 94));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DeviationToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal deviation)
            {
                if (deviation >= 0)
                    return new SolidColorBrush(Color.FromRgb(46, 204, 113));
                return new SolidColorBrush(Color.FromRgb(231, 76, 60));
            }
            return new SolidColorBrush(Color.FromRgb(52, 73, 94));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}