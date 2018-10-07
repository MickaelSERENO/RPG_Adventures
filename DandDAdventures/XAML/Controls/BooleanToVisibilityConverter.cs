using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DandDAdventures.XAML.Controls
{
    /// <summary>
    /// Class permitting to set the value of the visibility of a WPF object based on a boolean
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert function (implementation from IValueConverter
        /// </summary>
        /// <param name="value">The value to convert. Must be convertable to bool</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if(parameter != null)
            {
                if(parameter as string == "grid")
                {
                    if(b)
                        return "*";
                    return "0";
                }
            }
            if(b)
                return "Visible";
            return "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String s = value as String;
            if(s == "Visible")
                return true;
            return false;
        }
    }
}
