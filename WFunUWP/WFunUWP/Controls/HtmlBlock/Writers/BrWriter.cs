using System;
using WFunUWP.Models.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class BrWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "br" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            //LineBreak doesn't work with hyperlink
            return new Run
            {
                Text = Environment.NewLine
            };
        }
    }
}
