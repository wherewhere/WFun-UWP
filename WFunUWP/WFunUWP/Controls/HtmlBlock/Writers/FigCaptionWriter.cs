using WFunUWP.Models.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class FigCaptionWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "figcaption" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            return new Paragraph();
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlFragment fragment)
        {
            Paragraph caption = ctrl as Paragraph;
            caption.TextAlignment = Parse(style.Img.HorizontalAlignment);

            ApplyParagraphStyles(caption, style.FigCaption);
        }

        private static TextAlignment Parse(HorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    return TextAlignment.Left;
                case HorizontalAlignment.Right:
                    return TextAlignment.Right;
                default:
                    return TextAlignment.Center;
            }
        }
    }
}
