using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WFunUWP.Controls
{
    internal class GridRow : DependencyObject
    {
        public int Index { get; set; }
        public Grid Container { get; set; }
    }
}
