using System;
using System.Globalization;
using System.Windows.Data;

namespace ShutdownManager.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue;
            try
            {
                intValue = System.Convert.ToInt32(value);
            }
            catch (Exception)
            {

                intValue = 0;
            }


            return intValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value;    
   
        }
    }
}
