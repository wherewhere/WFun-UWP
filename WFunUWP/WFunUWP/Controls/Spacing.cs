using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WFunUWP.Controls
{
    public class Spacing
    {
        public static double GetHorizontal(DependencyObject obj)
        {
            return (double)obj.GetValue(HorizontalProperty);
        }

        public static double GetVertical(DependencyObject obj)
        {
            return (double)obj.GetValue(VerticalProperty);
        }

        private static void HorizontalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.StackPanel", "Spacing") && sender is StackPanel StackPanel)
            {
                StackPanel.Spacing = (double)e.NewValue;
            }
            else
            {
                double space = (double)e.NewValue;
                DependencyObject obj = (DependencyObject)sender;

                MarginSetter.SetMargin(obj, new Thickness(0, 0, space, 0));
                MarginSetter.SetLastItemMargin(obj, new Thickness(0));
            }
        }

        public static void SetHorizontal(DependencyObject obj, double space)
        {
            obj.SetValue(HorizontalProperty, space);
        }

        public static void SetVertical(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalProperty, value);
        }

        private static void VerticalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.StackPanel", "Spacing") && sender is StackPanel StackPanel)
            {
                StackPanel.Spacing = (double)e.NewValue;
            }
            else
            {
                double space = (double)e.NewValue;
                DependencyObject obj = (DependencyObject)sender;

                MarginSetter.SetMargin(obj, new Thickness(0, 0, 0, space));
                MarginSetter.SetLastItemMargin(obj, new Thickness(0));
            }
        }

        public static readonly DependencyProperty VerticalProperty =
            DependencyProperty.RegisterAttached("Vertical", typeof(double), typeof(Spacing),
                new PropertyMetadata(0d, VerticalChangedCallback));

        public static readonly DependencyProperty HorizontalProperty =
            DependencyProperty.RegisterAttached("Horizontal", typeof(double), typeof(Spacing),
                new PropertyMetadata(0d, HorizontalChangedCallback));
    }
}
