using HtmlAgilityPack;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace WFunUWP.Controls.Writers
{
    internal class ImageWriter : HtmlWriter
    {
        public override string[] TargetTags => new string[] { "img" };

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            HtmlNode node = fragment as HtmlNode;
            string src = GetImageSrc(node);
            if (node != null && !string.IsNullOrEmpty(src))
            {

                if (Uri.TryCreate(src, UriKind.Absolute, out Uri uri))
                {
                    try
                    {
                        Viewbox viewbox = CreateImage(node, src);

                        return IsInline(fragment)
                            ? new InlineUIContainer
                            {
                                Child = viewbox
                            }
                            : viewbox as DependencyObject;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading img@src '{uri?.ToString()}': {ex.Message}");
                    }
                }
            }
            return null;
        }

        private static bool IsInline(HtmlNode fragment)
        {
            return fragment.ParentNode != null && fragment.ParentNode.Name == "p";
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlNode fragment)
        {
            if (!IsInline(fragment))
            {
                ApplyImageStyles(ctrl as Viewbox, style.Img);
            }
        }

        private static Viewbox CreateImage(HtmlNode node, string src)
        {
            Viewbox viewbox = new Viewbox
            {
                StretchDirection = StretchDirection.DownOnly
            };

            int imgHeight = node.GetAttributeValue("height", 0);
            int width = node.GetAttributeValue("width", 0);

            if (imgHeight > 0)
            {
                viewbox.MaxHeight = imgHeight;
            }
            if (width > 0)
            {
                viewbox.MaxWidth = width;
            }

            viewbox.Child = new ImageEx
            {
                Source = src,
                Stretch = Stretch.Uniform,
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.Transparent)
            };
            return viewbox;
        }

        private static string GetImageSrc(HtmlNode img)
        {
            if (!string.IsNullOrEmpty(img.GetAttributeValue("src", null)))
            {
                return img.GetAttributeValue("src", null);
            }
            else if (!string.IsNullOrEmpty(img.GetAttributeValue("srcset", null)))
            {
                Regex regex = new Regex(@"(?:(?<src>[^\""'\s,]+)\s*(?:\s+\d+[wx])(?:,\s*)?)");
                MatchCollection matches = regex.Matches(img.GetAttributeValue("srcset", null));

                if (matches.Count > 0)
                {
                    Match m = matches[matches.Count - 1];
                    return m?.Groups["src"].Value;
                }
            }

            return null;
        }
    }
}
