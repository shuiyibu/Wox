using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Wox.Infrastructure.Image;

namespace Wox.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return null;
            }
            var image = ImageLoader.Load(value.ToString());
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}