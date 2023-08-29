using HtmlAgilityPack;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class AnchorWriter : HtmlWriter
    {
        public override string[] TargetTags => new string[] { "a" };

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            if (fragment.NodeType == HtmlNodeType.Element)
            {
                HtmlNode node = fragment;
                if (node != null)
                {
                    Hyperlink a = new Hyperlink();

                    if (Uri.TryCreate(node.GetAttributeValue("href", string.Empty), UriKind.Absolute, out Uri uri))
                    {
                        try
                        {
                            a.NavigateUri = uri;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error loading a@href '{uri?.ToString()}': {ex.Message}");
                        }
                    }

                    return a;
                }
            }
            return null;
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlNode fragment)
        {
            ApplyTextStyles(ctrl as Span, style.A);
        }
    }
}
