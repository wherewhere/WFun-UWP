using HtmlAgilityPack;
using System;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class UserModel : ICanCopy
    {
        public bool IsCopyEnabled { get; set; }
        public string UID { get; private set; }
        public string UserName { get; private set; }
        public ImageModel UserAvatar { get; private set; }

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
            if (token.TryGetNode("/div[3]", out HtmlNode uid)&& uid.InnerText.Contains("UID."))
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
