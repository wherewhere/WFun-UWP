using WFunUWP.Models.Html;
using Windows.UI.Xaml;

namespace WFunUWP.Controls.Writers
{
    internal class TableRowWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "tr" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            return new GridRow();
        }
    }
}
