using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WFunUWP.Models.Html;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace WFunUWP.Controls.Writers
{
    internal class ImageWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "img" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
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

        private static bool IsInline(HtmlFragment fragment)
        {
            return fragment.Parent != null && fragment.Parent.Name == "p";
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlFragment fragment)
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

            int imgHeight = node.Attributes.GetValueInt("height");
            int width = node.Attributes.GetValueInt("width");

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
            if (!string.IsNullOrEmpty(img.Attributes.GetValue("src")))
            {
                return img.Attributes.GetValue("src");
            }
            else if (!string.IsNullOrEmpty(img.Attributes.GetValue("srcset")))
            {
                Regex regex = new Regex(@"(?:(?<src>[^\""'\s,]+)\s*(?:\s+\d+[wx])(?:,\s*)?)");
                MatchCollection matches = regex.Matches(img.Attributes.GetValue("srcset"));

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
