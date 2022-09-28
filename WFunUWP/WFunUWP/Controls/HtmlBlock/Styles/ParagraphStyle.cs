using Windows.UI.Xaml;

namespace WFunUWP.Controls
{
    public class ParagraphStyle : TextStyle
    {
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register("Margin", typeof(Thickness), typeof(ParagraphStyle), new PropertyMetadata(new Thickness(double.NaN)));

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        public void Merge(ParagraphStyle style)
        {
            if (style != null)
            {
                Margin = Margin.Merge(style.Margin);

                base.Merge(style);
            }
        }
    }
}
