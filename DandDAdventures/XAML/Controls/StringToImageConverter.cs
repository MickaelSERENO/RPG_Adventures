using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DandDAdventures.XAML.Controls
{
    /// <summary>
    /// Converter used by the XAML view when setting a new image URI
    /// </summary>
    public class StringToImageConverter : IValueConverter
    {
        /// <summary>
        /// Constructor, does nothing
        /// </summary>
        public StringToImageConverter()
        { }

        /// <summary>
        /// Convert function (implementation from IValueConverter
        /// </summary>
        /// <param name="value">The value to convert. Must be convertable to string</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;
            String uri = value as string;

            if(uri != null)
            {
                BitmapImage image = BitmapFromPath(uri);
                result = image;
            }

            return result;
        }

        /// <summary>
        /// Load a BitmapImage from an pathfile
        /// </summary>
        /// <param name="path">The pathfile</param>
        /// <returns>The BitmapImage loaded</returns>
        public static BitmapImage BitmapFromPath(String path)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path);
            image.EndInit();

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
