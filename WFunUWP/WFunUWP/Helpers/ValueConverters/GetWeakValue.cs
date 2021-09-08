using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace WFunUWP.Helpers.ValueConverters
{
    public class GetWeakValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((string)parameter)
            {
                case "BitmapImage":
                    _ = ((WeakReference<BitmapImage>)value).TryGetTarget(out BitmapImage image);
                    return image == ImageCacheHelper.NoPic ? null : image;
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
    }
}
