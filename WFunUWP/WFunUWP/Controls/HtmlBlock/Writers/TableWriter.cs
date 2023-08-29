using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WFunUWP.Controls.Writers
{
    internal class TableWriter : HtmlWriter
    {
        public override string[] TargetTags => new string[] { "table" };

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            return new Grid();
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlNode fragment)
        {
            if (style?.Table != null)
            {
                Grid grid = ctrl as Grid;

                BindingOperations.SetBinding(grid, Grid.HorizontalAlignmentProperty, CreateBinding(style.Table, "HorizontalAlignment"));

                foreach (ColumnDefinition columnDefinition in grid.ColumnDefinitions)
                {
                    BindingOperations.SetBinding(columnDefinition, ColumnDefinition.WidthProperty, CreateBinding(style.Table, "ColumnWidth"));
                }

                ApplyContainerStyles(grid, style.Table);
            }
        }
    }
}
