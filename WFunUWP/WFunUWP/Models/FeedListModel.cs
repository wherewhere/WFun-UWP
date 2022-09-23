using HtmlAgilityPack;
using System;
using System.ComponentModel;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class FeedListModel : INotifyPropertyChanged, ICanCopy
    {
        private bool isCopyEnabled;
        public bool IsCopyEnabled
        {
            get => isCopyEnabled;
            set
            {
                if (isCopyEnabled != value)
                {
                    isCopyEnabled = value;
                    RaisePropertyChangedEvent();
                }
            }
        }

        public string Url { get; private set; }
        public string Uurl { get; private set; }
        public string Message { get; private set; }
        public string UserName { get; private set; }
        public string Dateline { get; private set; }
        public string DeviceTitle { get; private set; }
        public string MessageTitle { get; private set; }

        public RelationRowsItem RelationRows { get; private set; }

        public ImageModel UserAvatar { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public FeedListModel(string doc)
        {
            HtmlDocument token = new HtmlDocument();
            token.LoadHtml(doc);
            if (token.TryGetNode("/td/div/div/a", out HtmlNode uurl))
            {
                Uurl = uurl.GetAttributeValue("href", string.Empty).Trim();
            }
            if (token.TryGetNode("/td/div/div/a/img", out HtmlNode useravatar))
            {
                UserAvatar = new ImageModel(new Uri(UriHelper.BaseUri, useravatar.GetAttributeValue("src", string.Empty).Trim()).ToString(), ImageType.Avatar);
            }
            if (token.TryGetNode("/td/div/div[2]/div", out HtmlNode username))
            {
                UserName = username.InnerText.Trim();
            }
            if (token.TryGetNode("/td/div/div[2]/div[2]/span", out HtmlNode devicetitle))
            {
                DeviceTitle = devicetitle.InnerText.Trim();
            }
            if (token.TryGetNode("/td[2]/div/a", out HtmlNode messagetitle))
            {
                Url = messagetitle.GetAttributeValue("href", string.Empty);
                MessageTitle = messagetitle.InnerText.CSStoString().Trim();
            }
            if (token.TryGetNode("/td[2]/div[2]", out HtmlNode message))
            {
                Message = message.InnerText.CSStoString().Trim();
            }
            if (token.TryGetNode("/td[3]", out HtmlNode dateline))
            {
                Dateline = dateline.InnerText.Trim();
            }
            if (token.TryGetNode("/td[4]/a", out HtmlNode relationrows))
            {
                RelationRows = new RelationRowsItem
                {
                    Title = relationrows.InnerText.Trim(),
                    Url = relationrows.GetAttributeValue("href", string.Empty)
                };
            }
        }
    }

    public class RelationRowsItem
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
