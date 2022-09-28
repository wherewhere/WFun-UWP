using System;
using System.Diagnostics;
using WFunUWP.Models.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class AnchorWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "a" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            HtmlNode node = fragment as HtmlNode;
            if (node != null && node.Attributes.ContainsKey("href"))
            {
                Hyperlink a = new Hyperlink();


                if (Uri.TryCreate(node.Attributes["href"], UriKind.Absolute, out Uri uri))
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
            return null;
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlFragment fragment)
        {
            ApplyTextStyles(ctrl as Span, style.A);
        }
    }
}
