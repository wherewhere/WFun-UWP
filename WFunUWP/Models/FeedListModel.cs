using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class FeedListModel:ICanCopy
    {
        public string Url { get; private set; }
        public string Uurl { get; private set; }
        public string LikeNum { get; private set; }
        public string Message { get; private set; }
        public string ReplyNum { get; private set; }
        public string ShareNum { get; private set; }
        public string Username { get; private set; }
        public string Dateline { get; private set; }
        public string DeviceTitle { get; private set; }
        public string MessageTitle { get; private set; }

        public bool Liked { get; private set; }
        public bool IsCopyEnabled { get; set; }
        public bool ShowLikes { get; private set; }
        public bool ShowPicArr { get; private set; }
        public bool ShowRelationRows { get; private set; }

        public FeedListModel(string doc)
        {
            HtmlDocument token = new HtmlDocument();
            token.LoadHtml(doc);
            if (token.TryGetNode("/td[1]", out HtmlNode td1))
            {

            }
            if (token.TryGetNode("/td[2]", out HtmlNode td2))
            {
                MessageTitle = td2.ChildNodes[1].InnerText;
                Message = td2.ChildNodes[3].InnerText;
            }
            if (token.TryGetNode("/td[3]", out HtmlNode td3))
            {
                Dateline = td3.InnerText;
            }
        }
    }
}
