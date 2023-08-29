using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class PreWriter : HtmlWriter
    {
        public override string[] TargetTags => new string[] { "pre" };

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            return new Paragraph();
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlNode fragment)
        {
            ApplyParagraphStyles(ctrl as Paragraph, style.Pre);
        }
    }
}
