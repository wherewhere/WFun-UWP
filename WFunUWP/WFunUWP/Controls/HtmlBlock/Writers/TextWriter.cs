using HtmlAgilityPack;
using System;
using Windows.Data.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class TextWriter : HtmlWriter
    {
        public override string[] TargetTags => throw new NotImplementedException();

        public override bool Match(HtmlNode fragment)
        {
            return fragment.NodeType == HtmlNodeType.Text;
        }

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            if (fragment.NodeType == HtmlNodeType.Text)
            {
                HtmlNode text = fragment;
                return text != null && !string.IsNullOrEmpty(text.InnerText)
                    ? new Run
                    {
                        Text = HtmlUtilities.ConvertToText(text.InnerText)
                    }
                    : (DependencyObject)null;
            }
            return null;
        }
    }
}
