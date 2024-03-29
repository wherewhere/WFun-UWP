﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using WFunUWP.Helpers;
using WFunUWP.Helpers.DataSource;
using WFunUWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages.FeedPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ForumListPage : Page
    {
        internal AllForumDS AllForumDS = new AllForumDS();

        public ForumListPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AllForumDS.OnLoadMoreStarted += UIHelper.ShowProgressBar;
            AllForumDS.OnLoadMoreCompleted += UIHelper.HideProgressBar;
            _ = Refresh(-2);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AllForumDS.OnLoadMoreCompleted -= UIHelper.HideProgressBar;
            AllForumDS.OnLoadMoreStarted -= UIHelper.ShowProgressBar;
            base.OnNavigatedFrom(e);
        }

        private async Task Refresh(int p = -1)
        {
            if (p == -2)
            {
                await AllForumDS.Refresh();
            }
            else
            {
                _ = await AllForumDS.LoadMoreItemsAsync(20);
            }
        }

        internal static void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement s = sender as FrameworkElement;

            if (e != null) { e.Handled = true; }

            UIHelper.OpenLinkAsync(s.Tag as string);
        }

        internal static void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OnTapped(sender, e);
        }

        internal static void ListViewItem_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
            {
                OnTapped(sender, null);
            }
        }
    }

    internal class ForumListPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Null { get; set; }
        public DataTemplate List { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case ForumModel _: return List;
                default: return Null;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
    }

    /// <summary>
    /// Provide list of News. <br/>
    /// You can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more.
    /// </summary>
    internal class AllForumDS : DataSourceBase<object>
    {
        private int num = 1;

        protected override async Task<IList<object>> LoadItemsAsync(uint count)
        {
            if (_currentPage == 1)
            {
                num = 1;
            }
            List<object> Collection = new List<object>();
            int i = num;
            while (count-- > 0)
            {
                (bool isSucceed, HtmlDocument result) Results = await RequestHelper.GetHtmlAsync(UriHelper.GetUri(UriType.GetForumDetail, i++, 1));
                if (!Results.isSucceed || (Results.result.TryGetNode("/html/body/main/div/div", out HtmlNode error) && error.InnerText.Trim() == "版块已关闭"))
                {
                    if (i - 1 >= 110)
                    {
                        break;
                    }
                    else
                    {
                        if (Results.isSucceed)
                        {
                            await Task.Delay(10);// 防止 Wind 揍我。。。
                        }
                        Collection.Add(new NullModel());
                        continue;
                    }
                }
                if (Results.result.TryGetNode("/html/body/main/div/div/div/div", out HtmlNode node))
                {
                    Collection.Add(new ForumModel(node.InnerHtml)
                    {
                        Url = $"/forum-{i - 1}-1.html"
                    });
                }
                else
                {
                    Collection.Add(new NullModel());
                    continue;
                }
            }
            num += Collection.Count;
            return Collection;
        }

        protected override void AddItems(IList<object> items)
        {
            if (items != null)
            {
                foreach (object news in items)
                {
                    if (!(news is NullModel))
                    {
                        Add(news);
                    }
                }
            }
        }
    }

    internal class NullModel
    {
        public string Null { get; private set; }
    }
}
