using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers.DataSource;
using WFunUWP.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages.FeedPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class IndexPage : Page
    {
        internal NewsDS NewsDS = new NewsDS();

        public IndexPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _ = Refresh(-2);
        }

        private async Task Refresh(int p = -1)
        {
            if (p == -2)
            {
               await NewsDS.Refresh();
            }
            else
            {
                _ = await NewsDS.LoadMoreItemsAsync(20);
            }
        }
    }

    /// <summary>
    /// Provide list of News. <br/>
    /// You can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more.
    /// </summary>
    internal class NewsDS : DataSourceBase<object>
    {
        protected async override Task<IList<object>> LoadItemsAsync(uint count)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await NetworkHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetNewsFeeds, _currentPage)));
            HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/div/div/div/div/div/table/tbody");
            HtmlNodeCollection CNodes = node.ChildNodes;
            ObservableCollection<object> Collection = new ObservableCollection<object>();
            foreach (HtmlNode item in CNodes)
            {
                if (item.InnerHtml.Contains("td"))
                {
                    Collection.Add(new FeedListModel(item.InnerHtml));
                }
            }
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
