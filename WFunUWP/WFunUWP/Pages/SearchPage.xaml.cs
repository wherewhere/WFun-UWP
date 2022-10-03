using System;
using System.ComponentModel;
using System.Threading.Tasks;
using WFunUWP.Helpers;
using WFunUWP.Pages.FeedPages;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchPage : Page, INotifyPropertyChanged
    {
        private bool IsUseRegex = false;
        private bool IsSearchUser = false;
        private bool IsSearchTitle = true;
        private bool IsSearchContent = true;

        private readonly ResourceLoader _loader = ResourceLoader.GetForViewIndependentUse("SearchPage");

        private NewsDS newsDS;
        internal NewsDS NewsDS
        {
            get => newsDS;
            set
            {
                if (newsDS != value)
                {
                    newsDS = value;
                    RaisePropertyChangedEvent();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        public SearchPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is object[] vs && vs[0] is string word)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    SearchBox.Text = vs[0] as string;
                    NewsDS = new NewsDS(vs[0] as string);
                    NewsDS.OnLoadMoreStarted += UIHelper.ShowProgressBar;
                    NewsDS.OnLoadMoreCompleted += UIHelper.HideProgressBar;
                    UIHelper.MainPage.SetTitle(string.Format(_loader.GetString("Search"), NewsDS.SearchWord));
                    _ = Refresh(-2);
                }
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
            if (!string.IsNullOrWhiteSpace(sender.Text))
            {
                if (NewsDS == null)
                {
                    NewsDS = new NewsDS(sender.Text, new bool[] { IsSearchTitle, IsSearchContent, IsSearchUser, IsUseRegex });
                    NewsDS.OnLoadMoreStarted += UIHelper.ShowProgressBar;
                    NewsDS.OnLoadMoreCompleted += UIHelper.HideProgressBar;
                }
                else
                {
                    NewsDS?.Reset(sender.Text, new bool[] { IsSearchTitle, IsSearchContent, IsSearchUser, IsUseRegex });
                }
                UIHelper.MainPage.SetTitle(string.Format(_loader.GetString("Search"), NewsDS.SearchWord));
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
