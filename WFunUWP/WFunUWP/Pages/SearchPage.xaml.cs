using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WFunUWP.Helpers;
using WFunUWP.Pages.FeedPages;
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

namespace WFunUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        private bool isSearchUser = false;
        internal bool IsSearchUser
        {
            get => isSearchUser;
            set
            {
                isSearchUser = value;
                RaisePropertyChangedEvent();
            }
        }

        private bool isSearchTitle = true;
        internal bool IsSearchTitle
        {
            get => isSearchTitle;
            set
            {
                isSearchTitle = value;
                RaisePropertyChangedEvent();
            }
        }

        private bool isSearchForum = false;
        internal bool IsSearchForum
        {
            get => isSearchForum;
            set
            {
                isSearchForum = value;
                RaisePropertyChangedEvent();
            }
        }

        private bool isSearchContent = true;
        internal bool IsSearchContent
        {
            get => isSearchContent;
            set
            {
                isSearchContent = value;
                RaisePropertyChangedEvent();
            }
        }

        public SearchPage() => InitializeComponent();

        internal NewsDS NewsDS;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is object[] vs)
            {
                SearchBox.Text = vs[0] as string;
                NewsDS = new NewsDS(vs[0] as string);
                NewsDS.OnLoadMoreStarted += UIHelper.ShowProgressBar;
                NewsDS.OnLoadMoreCompleted += UIHelper.HideProgressBar;
                UIHelper.MainPage.SetTitle($"搜索:{NewsDS.SearchWord}");
                _ = Refresh(-2);
            }
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

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text != null)
            {
                NewsDS = NewsDS ?? new NewsDS();
                NewsDS.Reset(sender.Text, new object[] { IsSearchTitle, IsSearchContent, IsSearchUser, IsSearchForum });
                UIHelper.MainPage.SetTitle($"搜索:{NewsDS.SearchWord}");
                _ = Refresh(-2);
            }
        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SearchBox_QuerySubmitted(sender as AutoSuggestBox, null);
            }
        }
    }
}
