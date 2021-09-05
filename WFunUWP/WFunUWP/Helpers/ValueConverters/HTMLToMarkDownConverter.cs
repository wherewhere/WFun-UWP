using System;
using WFunUWP.Core.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WFunUWP.Helpers.ValueConverters
{
    public class HTMLToMarkDownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString().CSStoMarkDown();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => (Visibility)value == Visibility.Visible;
    }
}
