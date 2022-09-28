using WFunUWP.Models.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class StrongWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "strong", "b" }; }
        }


        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            return new Span();
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlFragment fragment)
        {
            ApplyTextStyles(ctrl as Span, style.Strong);
        }
    }
}
