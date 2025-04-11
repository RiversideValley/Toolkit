namespace Riverside.Toolkit.Converters;

public class ColorToSolidColorBrushConverter : IValueConverter
{
#if !Wpf
	public object Convert(object value, Type targetType, object parameter, string language)
#elif Wpf
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
		=> new SolidColorBrush((Color)value);

#if !Wpf
	public object ConvertBack(object value, Type targetType, object parameter, string language)
#elif Wpf
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        => ((SolidColorBrush)value).Color;
}
