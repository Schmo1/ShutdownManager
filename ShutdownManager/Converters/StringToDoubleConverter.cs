using System;
using System.Globalization;
using System.Windows.Data;

namespace ShutdownManager.Converters
{
    internal class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue;
            try
            {
                doubleValue = System.Convert.ToDouble(value);
            }
            catch (Exception)
            {

                doubleValue = 0;
            }


            return doubleValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();

        }
    }
}

