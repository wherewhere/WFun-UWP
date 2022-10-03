using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NewsDS.OnLoadMoreCompleted -= UIHelper.HideProgressBar;
            NewsDS.OnLoadMoreStarted -= UIHelper.ShowProgressBar;
            base.OnNavigatedFrom(e);
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
        private bool[] _vs;
        public string SearchWord;

        internal NewsDS(string searchword = null, bool[] vs = null)
        {
            SearchWord = searchword;
            _vs = vs;
        }

        internal void Reset(string searchword = null, bool[] vs = null)
        {
            SearchWord = searchword;
            _vs = vs;
        }

        protected override async Task<IList<object>> LoadItemsAsync(uint count)
        {
            List<object> Collection = new List<object>();
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
                    if (!string.IsNullOrWhiteSpace(SearchWord) && news is FeedListModel Feed)
                    {
                        if (_vs != null && _vs.Length >= 4)
                        {
                            if (_vs[3])
                            {
                                Regex regex = new Regex(SearchWord);
                                if ((_vs[0] && regex.IsMatch(Feed.MessageTitle)) || (_vs[1] && regex.IsMatch(Feed.Message)) || (_vs[2] && regex.IsMatch(Feed.UserName)))
                                {
                                    Add(news);
                                }
                            }
                            else
                            {
                                IEnumerable<string> list = SearchWord.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x));
                                foreach (string word in list)
                                {
                                    if ((_vs[0] && Feed.MessageTitle.Contains(word)) || (_vs[1] && Feed.Message.Contains(word)) || (_vs[2] && Feed.UserName.Contains(word)))
                                    {
                                        Add(news);
                                    }
                                }
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
