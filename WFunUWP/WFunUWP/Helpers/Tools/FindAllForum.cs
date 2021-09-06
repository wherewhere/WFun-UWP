using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers.DataSource;
using WFunUWP.Models;

namespace WFunUWP.Helpers.Tools
{
    internal static class FindAllForum
    {
        internal static async Task<ObservableCollection<object>> FindAll(int i, uint count)
        {
            ObservableCollection<object> Collection = new ObservableCollection<object>();
            while (true)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(await NetworkHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetForumDetail, i.ToString())));
                if (doc.TryGetNode("/html/body/main/div/div", out HtmlNode error) && error.InnerText.Trim() == "版块已关闭")
                {
                    break;
                }
                HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/div/div");
                Collection.Add(new ForumModel(node.InnerHtml));
                count--;
                if (count <= 0)
                {
                    break;
                }
                else
                {
                    i++;
                }
                await Task.Delay(2000);// 防止 Wind 揍我。。。
            }
            return Collection;
        }
    }

    /// <summary>
    /// Provide list of News. <br/>
    /// You can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more.
    /// </summary>
    internal class AllForumDS : DataSourceBase<object>
    {
        private int num = 1;

        protected async override Task<IList<object>> LoadItemsAsync(uint count)
        {
            if (_currentPage == 1)
            {
                num = 1;
            }
            ObservableCollection<object> Collection = await FindAllForum.FindAll(num, count);
            num += Collection.Count;
            return Collection;
        }

        protected override void AddItems(IList<object> items)
        {
            if (items != null)
            {
                foreach (object news in items)
                {
                    Add(news);
                }
            }
        }
    }
}
