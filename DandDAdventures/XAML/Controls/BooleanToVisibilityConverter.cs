using System;
using System.Globalization;
using System.Windows.Data;

namespace DandDAdventures.XAML.Controls
{
    /// <summary>
    /// Class permitting to set the value of the visibility of a WPF object based on a boolean
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert function (implementation from IValueConverter).
        /// </summary>
        /// <param name="value">The value to convert. Must be convertable to bool</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use. If parameter starts by "grid", we use the following chracter for true or "0" for false</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
            if(parameter != null)
            {
                if((parameter as string).StartsWith("grid"))
                {
                    if(b)
                        return (parameter as string).Substring(4);
                    return "0";
                }
            }
            if(b)
                return "Visible";
            return "Collapsed";
        }

        /// <summary>
        /// Convert back a Visibility value to a Boolean
        /// </summary>
        /// <param name="value">The value to convert. Must be convertable to bool</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use. If parameter starts by "grid", we use the following chracter for true or "0" for false</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A boolean based on the Visibility Value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String s = value as String;
            if(s == "Visible")
                return true;
            return false;
        }
    }
}
