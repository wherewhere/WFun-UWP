using HtmlAgilityPack;
using System;
using System.Collections.ObjectModel;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class FeedDetailModel : ICanCopy
    {
        public string UID { get; private set; }
        public string Uurl { get; private set; }
        public string QRUrl { get; private set; }
        public string Message { get; private set; }
        public string UserName { get; private set; }
        public string Dateline { get; private set; }
        public string ForumUrl { get; private set; }
        public string ForumTitle { get; private set; }
        public string MessageTitle { get; private set; }
        public string ReportMessage { get; private set; }
        public ObservableCollection<FeedReplyModel> ReplyList { get; private set; }
        public ObservableCollection<RelationRowsItem> RelationRows { get; private set; }

        public bool IsCopyEnabled { get; set; }
        public bool IsFeedArticle { get; private set; }
        public bool ShowRelationRows { get; private set; }

        public ImageModel UserAvatar { get; private set; }

        public FeedDetailModel(string doc)
        {
            HtmlDocument token = new HtmlDocument();
            token.LoadHtml(doc);
            if (token.TryGetNode("/div/nav/a[2]", out HtmlNode forumtitle))
            {
                ForumTitle = forumtitle.InnerText.Trim();
                ForumUrl = forumtitle.GetAttributeValue("href", string.Empty).Trim();
            }
            if (token.TryGetNode("/div/nav/a[3]", out HtmlNode qrurl))
            {
                QRUrl = new Uri(UriHelper.BaseUri, qrurl.GetAttributeValue("href", string.Empty).Trim()).ToString();
            }
            if (token.TryGetNode("/div[2]/div", out HtmlNode messagetitle))
            {
                MessageTitle = messagetitle.InnerText.CSStoString().Trim();
            }
            if (token.TryGetNode("/div[3]/div/div/a/img", out HtmlNode useravatar))
            {
                UserAvatar = new ImageModel(new Uri(UriHelper.BaseUri, useravatar.GetAttributeValue("src", string.Empty).Trim()).ToString(), ImageType.Avatar);
            }
            if (token.TryGetNode("/div[3]/div/div[2]/div/a", out HtmlNode username))
            {
                UserName = username.InnerText.Trim();
                Uurl = username.GetAttributeValue("href", string.Empty).Trim();
            }
            if (token.TryGetNode("/div[3]/div/div[2]/div/span", out HtmlNode uid))
            {
                UID = uid.InnerText.Trim();
            }
            if (token.TryGetNode("/div[3]/div/div[2]/div[2]", out HtmlNode dateline))
            {
                Dateline = dateline.InnerText.Trim();
            }
            if (token.TryGetNode("/div[3]/div[2]", out HtmlNode message))
            {
                Message = message.InnerHtml.Trim();
                IsFeedArticle = message.InnerText.Length >= 200;
            }
            else if (token.TryGetNode("/div/div/div/div", out message))
            {
                Message = message.InnerHtml.Trim();
                IsFeedArticle = message.InnerText.Length >= 200;
            }
            if (token.TryGetNode("/div[3]/div[3]/p", out HtmlNode tags))
            {
                RelationRows = new ObservableCollection<RelationRowsItem>();
                foreach (HtmlNode item in tags.ChildNodes)
                {
                    if (item.HasChildNodes && item.Name == "a")
                    {
                        RelationRows.Add(new RelationRowsItem()
                        {
                            Url = item.GetAttributeValue("href", string.Empty).Trim(),
                            Title = item.InnerText.Trim()
                        });
                    }
                }
                ShowRelationRows = RelationRows.Count > 0;
                if (token.TryGetNode("/div[3]/div[4]/div", out HtmlNode reportmessage))
                {
                    ReportMessage = reportmessage.InnerHtml.Trim();
                }
            }
            else if (token.TryGetNode("/div[3]/div[3]/div", out HtmlNode reportmessage))
            {
                ReportMessage = reportmessage.InnerHtml.Trim();
            }
            if (token.TryGetNode("/div[4]", out HtmlNode replylist))
            {
                ReplyList = new ObservableCollection<FeedReplyModel>();
                foreach (HtmlNode item in replylist.ChildNodes)
                {
                    if (item.HasChildNodes && !(item.InnerHtml == "全部回复："))
                    {
                        ReplyList.Add(new FeedReplyModel(item.InnerHtml));
                    }
                }
            }
            else if (token.TryGetNode("/div/div/div[2]/div", out replylist))
            {
                ReplyList = new ObservableCollection<FeedReplyModel>();
                foreach (HtmlNode item in replylist.ChildNodes)
                {
                    if (item.HasChildNodes && !(item.InnerHtml == "全部回复："))
                    {
                        ReplyList.Add(new FeedReplyModel(item.InnerHtml));
                    }
                }
            }
        }
    }
}
