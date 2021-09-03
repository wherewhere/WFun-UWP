using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFunUWP.Helpers
{
    internal static class DataHelper
    {
        public static bool TryGetNode(this HtmlDocument HTML, string XPath, out HtmlNode Node)
        {
            Node = HTML.DocumentNode.SelectSingleNode(XPath);
            return Node != null;
        }
    }
}
