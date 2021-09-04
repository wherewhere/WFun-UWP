using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;

namespace WFunUWP.Models
{
    public class ForumModel : ICanCopy
    {
        public bool IsCopyEnabled { get; set; }
        public string Title { get; private set; }
        public string SubTitle { get; private set; }
        public ImageModel Logo { get; private set; }

        public ForumModel(string doc)
        {
            HtmlDocument token = new HtmlDocument();
            token.LoadHtml(doc);
            if (token.TryGetNode("/div/img", out HtmlNode logo))
            {
                Logo = new ImageModel(new Uri(UriHelper.BaseUri, logo.GetAttributeValue("src", string.Empty).Trim()).ToString(), ImageType.Avatar);
            }
            if (token.TryGetNode("/div[2]/div", out HtmlNode title))
            {
                Title = title.InnerText.Trim();
            }
            if (token.TryGetNode("/div[2]/div[2]", out HtmlNode subtitle))
            {
                SubTitle = subtitle.InnerText.Trim();
            }
        }
    }
}
