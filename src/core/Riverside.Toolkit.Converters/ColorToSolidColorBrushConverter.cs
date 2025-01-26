namespace Riverside.Toolkit.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => new SolidColorBrush((Color)value);

        public object ConvertBack(object value, Type targetType, object parameter, string language) => ((SolidColorBrush)value).Color;
    }
}
