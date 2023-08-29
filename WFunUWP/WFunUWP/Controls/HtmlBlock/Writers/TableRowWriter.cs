using HtmlAgilityPack;
using Windows.UI.Xaml;

namespace WFunUWP.Controls.Writers
{
    internal class TableRowWriter : HtmlWriter
    {
        public override string[] TargetTags => new string[] { "tr" };

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            return new GridRow();
        }
    }
}
