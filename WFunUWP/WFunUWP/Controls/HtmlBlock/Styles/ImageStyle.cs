using Windows.UI.Xaml;

namespace WFunUWP.Controls
{
    public class ImageStyle : DependencyObject
    {
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register("Margin", typeof(Thickness), typeof(ImageStyle), new PropertyMetadata(new Thickness(double.NaN)));

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(ImageStyle), new PropertyMetadata(HorizontalAlignment.Stretch));

        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public void Merge(ImageStyle style)
        {
            if (style != null)
            {
                Margin = Margin.Merge(style.Margin);
                if (HorizontalAlignment != style.HorizontalAlignment)
                {
                    HorizontalAlignment = style.HorizontalAlignment;
                }
            }
        }
    }
}
