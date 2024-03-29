﻿using HtmlAgilityPack;
using System;
using System.ComponentModel;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class FeedReplyModel : ICanCopy, INotifyPropertyChanged
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

        public string UID { get; private set; }
        public string Uurl { get; private set; }
        public string Message { get; private set; }
        public string UserName { get; private set; }
        public string Dateline { get; private set; }

        public ImageModel UserAvatar { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public FeedReplyModel(string doc)
        {
            HtmlDocument token = new HtmlDocument();
            token.LoadHtml(doc);
            if (token.TryGetNode("/div/div/a/img", out HtmlNode useravatar))
            {
                UserAvatar = new ImageModel(new Uri(UriHelper.BaseUri, useravatar.GetAttributeValue("src", string.Empty).Trim()).ToString(), ImageType.Avatar);
            }
            if (token.TryGetNode("/div/div[2]/div/a", out HtmlNode username))
            {
                UserName = username.InnerText.Trim();
                Uurl = username.GetAttributeValue("href", string.Empty).Trim();
            }
            if (token.TryGetNode("/div/div[2]/div/span", out HtmlNode uid))
            {
                UID = uid.InnerText.Trim();
            }
            if (token.TryGetNode("/div/div[2]/div[2]", out HtmlNode dateline))
            {
                Dateline = dateline.InnerText.Trim();
            }
            if (token.TryGetNode("/div[2]", out HtmlNode message))
            {
                Message = message.InnerHtml.Trim();
            }
        }
    }
}
