using WFunUWP.Models.Html;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

namespace WFunUWP.Controls.Writers
{
    internal class TextWriter : HtmlWriter
    {
        public override string[] TargetTags
        {
            get { return new string[] { "text" }; }
        }

        public override DependencyObject GetControl(HtmlFragment fragment)
        {
            HtmlText text = fragment?.AsText();
            return text != null && !string.IsNullOrEmpty(text.Content)
                ? new Run
                {
                    Text = text.Content
                }
                : (DependencyObject)null;
        }
    }
}
