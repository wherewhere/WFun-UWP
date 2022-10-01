using WFunUWP.Helpers;
using WFunUWP.Models.Html;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace WFunUWP.Controls.Writers
{
    internal class TableDataWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "td", "th" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            return new GridColumn()
            {
                ColSpan = GetSpan(fragment.AsNode(), "colspan"),
                RowSpan = GetSpan(fragment.AsNode(), "rowspan"),
            };
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlFragment fragment)
        {
            GridColumn column = ctrl as GridColumn;
            Border border = column.Row.Container.GetChild<Border>(column.Index, column.Row.Index);
            if (border != null)
            {
                int? tableBorder = GetTableBorder(fragment.AsNode());
                if (tableBorder.HasValue)
                {
                    border.BorderThickness = new Thickness(tableBorder.Value);
                    border.BorderBrush = style.Table?.BorderForeground ?? new SolidColorBrush(Colors.Black);
                }
                else if (style.Table != null && !double.IsNaN(style.Table.Border.Top))
                {
                    BindingOperations.SetBinding(border, Border.BorderThicknessProperty, CreateBinding(style.Table, "Border"));
                    BindingOperations.SetBinding(border, Border.BorderBrushProperty, CreateBinding(style.Table, "BorderForeground"));
                }

                if (style.Table != null)
                {
                    BindingOperations.SetBinding(border, Border.MarginProperty, CreateBinding(style.Td, "Margin"));
                    BindingOperations.SetBinding(border, Border.PaddingProperty, CreateBinding(style.Td, "Padding"));
                }
            }
        }

        private static int GetSpan(HtmlNode node, string name)
        {
            return node != null ? node.Attributes.GetValueInt(name) : 0;
        }

        private static int? GetTableBorder(HtmlNode node)
        {
            HtmlNode table = node?.Ascendant("table") as HtmlNode;

            return table != null && !string.IsNullOrEmpty(table.Attributes.GetValue("border")) ? table.Attributes.GetValueInt("border") : null as int?;
        }
    }
}
