using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class FeedModelBase
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Dateline { get; private set; }

        public FeedModelBase(string doc)
        {
            HtmlDocument token = new HtmlDocument();
            token.LoadHtml(doc);
            if (token.TryGetNode("/td[1]", out HtmlNode td1))
            {

            }
            if (token.TryGetNode("/td[2]", out HtmlNode td2))
            {
                Title = td2.ChildNodes[1].InnerText;
                Message = td2.ChildNodes[3].InnerText;
            }
            if (token.TryGetNode("/td[3]", out HtmlNode td3))
            {
                Dateline = td3.InnerText;
            }
        }
    }
}
