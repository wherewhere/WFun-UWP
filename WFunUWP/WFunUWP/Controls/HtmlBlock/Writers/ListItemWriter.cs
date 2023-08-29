using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class ListItemWriter : HtmlWriter
    {
        public override string[] TargetTags => new string[] { "li" };

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            return new Paragraph();
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlNode fragment)
        {
            Paragraph li = ctrl as Paragraph;
            ListStyle currentStyle = style.Li;

            if (!string.IsNullOrEmpty(currentStyle?.Bullet))
            {
                currentStyle.RegisterPropertyChangedCallback(ListStyle.BulletProperty, (sender, dp) =>
                {
                    Run r = li.Inlines[0] as Run;
                    if (r != null)
                    {
                        r.Text = currentStyle?.Bullet;
                    }
                });

                li.Inlines.Insert(0, new Run
                {
                    Text = currentStyle.Bullet
                });
                li.Inlines.Insert(1, new Run
                {
                    Text = " "
                });
            }
            ApplyParagraphStyles(li, currentStyle);
        }
    }
}
