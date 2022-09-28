using System.Net;

namespace WFunUWP.Models.Html
{
    public sealed class HtmlText : HtmlFragment
    {
        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = WebUtility.HtmlDecode(value);
            }
        }

        public HtmlText()
        {
            Name = "text";
        }

        public HtmlText(string doc, int startIndex, int endIndex) : this()
        {
            if (!string.IsNullOrEmpty(doc) && endIndex - startIndex > 0)
            {
                Content = doc.Substring(startIndex, endIndex - startIndex);
            }
        }

        public override string ToString()
        {
            return Content;
        }

        internal static HtmlText Create(string doc, HtmlTag startTag, HtmlTag endTag)
        {
            int startIndex = 0;

            if (startTag != null)
            {
                startIndex = startTag.StartIndex + startTag.Length;
            }

            int endIndex = doc.Length;

            if (endTag != null)
            {
                endIndex = endTag.StartIndex;
            }

            HtmlText text = new HtmlText(doc, startIndex, endIndex);
            return text != null && !string.IsNullOrEmpty(text.Content) ? text : null;
        }
    }
}
