using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
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
            if(vs[0] is string id && vs[1] is FeedListType type)
            {
                ForumDS = new ForumDS(id, type);
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

        public static void UserDetailBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //if (!(e == null || UIHelper.IsOriginSource(sender, e.OriginalSource))) { return; }
            //if (e.OriginalSource.GetType() == typeof(Windows.UI.Xaml.Shapes.Ellipse)) { return; }
            //if (sender is ListViewItem l && l.Tag is Models.IndexPageModel i)
            //{
            //    UIHelper.OpenLinkAsync(i.Url);
            //}
            //else { return; }

            //UIHelper.ShowImage((sender as FrameworkElement)?.Tag as ImageModel);
        }

        internal static void UserDetailBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Border b = sender as Border;
            b.Height = e.NewSize.Width <= 400 ? e.NewSize.Width : 400;
        }
    }

    internal class FeedListPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Feed { get; set; }
        public DataTemplate UserHeader { get; set; }
        public DataTemplate ForumHeader { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case UserModel _: return UserHeader;
                case ForumModel _: return ForumHeader;
                default: return Feed;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
    }

    public enum FeedListType
    {
        User,
        Forum,
    }

    internal class ForumDS : DataSourceBase<object>
    {
        private string _id;
        private FeedListType _type;

        internal ForumDS(string id, FeedListType type)
        {
            _id = id;
            _type = type;
        }

        protected async override Task<IList<object>> LoadItemsAsync(uint count)
        {
            HtmlDocument doc = new HtmlDocument();
            ObservableCollection<object> Collection = new ObservableCollection<object>();
            switch (_type)
            {
                case FeedListType.User:
                    if (_currentPage == 1)
                    {
                        doc.LoadHtml(await NetworkHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetUserDetail, _id)));
                    }
                    break;
                case FeedListType.Forum:
                    doc.LoadHtml(await NetworkHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetForumDetail, _id, _currentPage)));
                    break;
                default:
                    break;
            }
            if (_currentPage == 1)
            {
                HtmlNode head = doc.DocumentNode.SelectSingleNode("/html/body/main/div/div/div/div");
                switch (_type)
                {
                    case FeedListType.User: Collection.Add(new UserModel(head.InnerHtml)); break;
                    case FeedListType.Forum: Collection.Add(new ForumModel(head.InnerHtml)); break;
                    default: break;
                }
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
