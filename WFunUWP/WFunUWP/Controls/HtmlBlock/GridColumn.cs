using Windows.UI.Xaml;

namespace WFunUWP.Controls
{
    internal class GridColumn : DependencyObject
    {
        public int Index { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public GridRow Row { get; set; }
    }
}
