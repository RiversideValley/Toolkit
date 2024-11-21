using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Cube.UI.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => new SolidColorBrush((Color)value);

        public object ConvertBack(object value, Type targetType, object parameter, string language) => ((SolidColorBrush)value).Color;
    }
}
