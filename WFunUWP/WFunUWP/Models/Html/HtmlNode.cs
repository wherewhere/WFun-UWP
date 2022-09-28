using System.Collections.Generic;

namespace WFunUWP.Models.Html
{
    public sealed class HtmlNode : HtmlFragment
    {
        public Dictionary<string, string> Attributes { get; }

        internal HtmlNode(HtmlTag openTag)
        {
            Name = openTag.Name.ToLowerInvariant();
            Attributes = openTag.Attributes;
        }
    }
}
