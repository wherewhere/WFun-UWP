using HtmlAgilityPack;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Text.RegularExpressions;

namespace WFunUWP.Controls.Writers
{
    internal class YouTubeWriter : IFrameVideoWriter
    {
        public override string[] TargetTags => throw new NotImplementedException();

        public override bool Match(HtmlNode fragment)
        {
            if (fragment == null || fragment.NodeType != HtmlNodeType.Element) { return false; }
            string src = GetIframeSrc(fragment);
            return fragment.Name == "iframe" && !string.IsNullOrWhiteSpace(src) && src.ToLowerInvariant().Contains("www.youtube.com");
        }

        protected override ImageStyle GetStyle(DocumentStyle style)
        {
            return style.YouTube;
        }

        protected override void SetScreenshot(ImageEx img, HtmlNode node)
        {
            img.Source = GetImageSrc(node);
        }

        private static string GetImageSrc(HtmlNode node)
        {
            Regex regex = new Regex(@"(?:http(?:s?)://www\.youtube\.com/embed/)(?<videoid>[\w_-]*)(?:\??)(?:/?)(?:\.*)");
            string src = GetIframeSrc(node);
            if (!string.IsNullOrEmpty(src))
            {
                Match match = regex.Match(src);
                if (match.Success)
                {
                    return $"https://i.ytimg.com/vi/{match.Groups["videoid"].Value}/maxresdefault.jpg";
                }
            }
            return null;
        }
    }
}
