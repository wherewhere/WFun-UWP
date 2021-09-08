using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace WFunUWP.Helpers
{
    internal static class DataHelper
    {
        public static bool TryGetNode(this HtmlDocument HTML, string XPath, out HtmlNode Node)
        {
            Node = HTML.DocumentNode.SelectSingleNode(XPath);
            return Node != null;
        }

        public static string HTMLEntitytoNormal(this string strformat)
        {
            string regx = "(?<=(& #)).+?(?=;)";
            MatchCollection matchCol = Regex.Matches(strformat, regx);
            if (matchCol.Count > 0)
            {
                for (int i = 0; i < matchCol.Count; i++)
                {
                    int asciinum = int.Parse(matchCol[i].Value);
                    char c = (char)asciinum;
                    strformat = strformat.Replace(string.Format("& #{0};", asciinum), c.ToString());
                }
            }
            return strformat;
        }
    }
}
