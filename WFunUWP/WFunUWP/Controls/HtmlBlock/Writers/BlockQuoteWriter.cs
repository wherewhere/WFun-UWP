﻿using WFunUWP.Models.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class BlockQuoteWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "blockquote" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            return new Paragraph();
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlFragment fragment)
        {
            ApplyParagraphStyles(ctrl as Paragraph, style.BlockQuote);
        }
    }
}
