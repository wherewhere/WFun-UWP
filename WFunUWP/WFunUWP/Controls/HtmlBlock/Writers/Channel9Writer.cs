using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using WFunUWP.Models.Html;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace WFunUWP.Controls.Writers
{
    internal class Channel9Writer : IFrameVideoWriter
    {
        public override string[] TargetTags
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Match(HtmlFragment fragment)
        {
            string src = GetIframeSrc(fragment);
            return fragment.Name == "iframe" && !string.IsNullOrWhiteSpace(src) && src.ToLowerInvariant().Contains("channel9.msdn.com");
        }

        protected override ImageStyle GetStyle(DocumentStyle style)
        {
            return style.Channel9;
        }

        protected override void SetScreenshot(ImageEx img, HtmlNode node)
        {
            img.Source = GetEmbebedImage("WFunUWP.Controls.HtmlBlock.channel9-screen.png");
            img.Background = new SolidColorBrush(Color.FromArgb(255, 249, 203, 66));
        }
    }
}
