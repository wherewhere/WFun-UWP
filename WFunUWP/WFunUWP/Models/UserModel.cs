using HtmlAgilityPack;
using System;
using System.ComponentModel;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class UserModel : ICanCopy, INotifyPropertyChanged
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
        public string UserName { get; private set; }
        public ImageModel UserAvatar { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public UserModel(string doc)
        {
            HtmlDocument token = new HtmlDocument();
            token.LoadHtml(doc);
            if (token.TryGetNode("/div/img", out HtmlNode useravatar))
            {
                UserAvatar = new ImageModel(new Uri(UriHelper.BaseUri, useravatar.GetAttributeValue("src", string.Empty).Trim()).ToString(), ImageType.Avatar);
            }
            if (token.TryGetNode("/div[2]", out HtmlNode username))
            {
                UserName = username.InnerText.Trim();
            }
            if (token.TryGetNode("/div[3]", out HtmlNode uid) && uid.InnerText.Contains("UID."))
            {
                UID = uid.InnerText.Trim().Replace("UID.", string.Empty);
            }
            else if (token.TryGetNode("/div[4]", out HtmlNode myuid))
            {
                UID = myuid.InnerText.Trim().Replace("UID.", string.Empty);
            }
        }
    }
}
