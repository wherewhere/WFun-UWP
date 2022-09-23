using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
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
            NewsDS.OnLoadMoreStarted += UIHelper.ShowProgressBar;
            NewsDS.OnLoadMoreCompleted += UIHelper.HideProgressBar;
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
        private object[] _vs;
        public string SearchWord;

        internal NewsDS(string searchword = null, object[] vs = null)
        {
            SearchWord = searchword;
            _vs = vs;
        }

        internal void Reset(string searchword = null, object[] vs = null)
        {
            SearchWord = searchword;
            _vs = vs;
        }

        protected override async Task<IList<object>> LoadItemsAsync(uint count)
        {
            ObservableCollection<object> Collection = new ObservableCollection<object>();
            (bool isSucceed, HtmlDocument result) Results = await RequestHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetNewsFeeds, _currentPage));
            if (Results.isSucceed && Results.result.TryGetNode("/html/body/main/div/div/div/div/div/div/div/table/tbody", out HtmlNode node) && node.HasChildNodes)
            {
                HtmlNodeCollection CNodes = node.ChildNodes;
                foreach (HtmlNode item in CNodes)
                {
                    if (item.HasChildNodes)
                    {
                        Collection.Add(new FeedListModel(item.InnerHtml));
                    }
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
                    if (SearchWord != null && news is FeedListModel Feed)
                    {
                        if (_vs != null)
                        {
                            if (((bool)_vs[0] && Feed.MessageTitle.Contains(SearchWord)) || ((bool)_vs[1] && Feed.Message.Contains(SearchWord)) || ((bool)_vs[2] && Feed.UserName.Contains(SearchWord)) || ((bool)_vs[3] && Feed.RelationRows.Title.Contains(SearchWord)))
                            {
                                Add(news);
                            }
                        }
                        else
                        {
                            if (Feed.MessageTitle.Contains(SearchWord) || Feed.Message.Contains(SearchWord))
                            {
                                Add(news);
                            }
                        }
                    }
                    else
                    {
                        Add(news);
                    }
                }
            }
        }
    }
}
