﻿using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class EmWriter : HtmlWriter
    {
        public override string[] TargetTags => new string[] { "em" };

        public override DependencyObject GetControl(HtmlNode fragment)
        {
            return new Span();
        }

        public override void ApplyStyles(DocumentStyle style, DependencyObject ctrl, HtmlNode fragment)
        {
            ApplyTextStyles(ctrl as Span, style.Em);
        }
    }
}
