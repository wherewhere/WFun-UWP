using System;
using Windows.UI.Xaml.Data;

namespace WFunUWP.Helpers.ValueConverters
{
    public class HasValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((string)parameter)
            {
                case "string": return !string.IsNullOrEmpty((string)value);
                default: return value != null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }
}
