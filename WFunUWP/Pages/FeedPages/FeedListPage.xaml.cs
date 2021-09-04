using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers.DataSource;
using WFunUWP.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages.FeedPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FeedListPage : Page
    {
        internal ForumDS ForumDS;

        public FeedListPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object[] vs = e.Parameter as object[];
            if(vs[0] is string id)
            {
                ForumDS = new ForumDS(id);
            }
            _ = Refresh(-2);
        }

        private async Task Refresh(int p = -1)
        {
            if (p == -2)
            {
                await ForumDS.Refresh();
            }
            else
            {
                _ = await ForumDS.LoadMoreItemsAsync(20);
            }
        }
    }

    internal class FeedListPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Feed { get; set; }
        public DataTemplate ForumHeader { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case ForumModel _: return ForumHeader;
                default: return Feed;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
    }

    internal class ForumDS : DataSourceBase<object>
    {
        private string _id;

        internal ForumDS(string id)
        {
            _id = id;
        }

        protected async override Task<IList<object>> LoadItemsAsync(uint count)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await NetworkHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetForumDetail, _id, _currentPage)));
            ObservableCollection<object> Collection = new ObservableCollection<object>();
            if (_currentPage == 1)
            {
                HtmlNode head = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/div/div");
                Collection.Add(new ForumModel(head.InnerHtml));
            }
            HtmlNode node = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/div/div[2]/div/div/div/table/tbody");
            HtmlNodeCollection CNodes = node.ChildNodes;
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
