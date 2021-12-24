using System;
using System.Collections.Generic;
using System.Linq;
using WFunUWP.Helpers;
using WFunUWP.Helpers.Tasks;
using WFunUWP.Pages.FeedPages;
using WFunUWP.Pages.SettingsPages;
using Windows.ApplicationModel.Core;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("Home", typeof(HomePage)),
            ("Chat", typeof(IndexPage)),
            ("Find", typeof(SearchPage)),
            ("List", typeof(ForumListPage)),
        };

        public MainPage()
        {
            InitializeComponent();
            UIHelper.MainPage = this;
            LiveTileTask.UpdateTile();
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                Window.Current.SetTitleBar(null);
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }
            RectanglePointerExited();
            UIHelper.CheckTheme();
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            NavigationViewFrame.Navigated += On_Navigated;
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        private void NavigationView_Navigate(string NavItemTag, NavigationTransitionInfo TransitionInfo, object[] vs = null)
        {
            Type _page = null;
            if (NavItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                (string Tag, Type Page) item = _pages.FirstOrDefault(p => p.Tag.Equals(NavItemTag, StringComparison.Ordinal));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            Type PreNavPageType = NavigationViewFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Equals(PreNavPageType, _page))
            {
                _ = NavigationViewFrame.Navigate(_page, vs, TransitionInfo);
            }
        }

        private void NavigationView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args) => _ = TryGoBack();

        private void NavigationView_SelectionChanged(muxc.NavigationView sender, muxc.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                _ = NavigationViewFrame.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                string NavItemTag = args.SelectedItemContainer.Tag.ToString();
                NavigationView_Navigate(NavItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private bool TryGoBack()
        {
            if (!NavigationViewFrame.CanGoBack)
            { return false; }

            // Don't go back if the nav pane is overlayed.
            if (NavigationView.IsPaneOpen &&
                (NavigationView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavigationView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
            { return false; }

            NavigationViewFrame.GoBack();
            return true;
        }

        private void On_Navigated(object _, NavigationEventArgs e)
        {
            NavigationView.IsBackEnabled = NavigationViewFrame.CanGoBack;
            if (NavigationViewFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavigationView.SelectedItem = (muxc.NavigationViewItem)NavigationView.SettingsItem;
                SetTitle("设置");
            }
            else if (NavigationViewFrame.SourcePageType == typeof(FeedShellPage))
            {
                SetTitle("动态");
            }
            else if (NavigationViewFrame.SourcePageType == typeof(FeedListPage))
            {
                SetTitle("论坛");
            }
            else if (NavigationViewFrame.SourcePageType == typeof(TestPage))
            {
                SetTitle("测试");
            }
            else if (NavigationViewFrame.SourcePageType != null)
            {
                (string Tag, Type Page) item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
                try
                {
                    NavigationView.SelectedItem = NavigationView.MenuItems
                        .OfType<muxc.NavigationViewItem>()
                        .First(n => n.Tag.Equals(item.Tag));
                }
                catch
                {
                    try
                    {
                        NavigationView.SelectedItem = NavigationView.FooterMenuItems
                            .OfType<muxc.NavigationViewItem>()
                            .First(n => n.Tag.Equals(item.Tag));
                    }
                    catch { }
                }
                SetTitle(((muxc.NavigationViewItem)NavigationView.SelectedItem)?.Content?.ToString());
            }
        }

        public void SetTitle(string Title) => HeaderTitle.Text = Title;

        #region 状态栏
        public void ShowProgressBar()
        {
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.IsIndeterminate = true;
            ProgressBar.ShowError = false;
            ProgressBar.ShowPaused = false;
        }

        public void PausedProgressBar()
        {
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.IsIndeterminate = true;
            ProgressBar.ShowError = false;
            ProgressBar.ShowPaused = true;
        }

        public void ErrorProgressBar()
        {
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.IsIndeterminate = true;
            ProgressBar.ShowPaused = false;
            ProgressBar.ShowError = true;
        }

        public void HideProgressBar()
        {
            ProgressBar.Visibility = Visibility.Collapsed;
            ProgressBar.IsIndeterminate = false;
            ProgressBar.ShowError = false;
            ProgressBar.ShowPaused = false;
        }

        public void ShowMessage(string message, int num, InfoType type)
        {
            Message.Text = message;
            MessageInfo.Value = num;
            switch(type)
            {
                case InfoType.Success:
                    MessageInfo.Style = (Style)Application.Current.Resources["SuccessIconInfoBadgeStyle"];
                    break;
                case InfoType.Critical:
                    MessageInfo.Style = (Style)Application.Current.Resources["CriticalIconInfoBadgeStyle"];
                    break;
                case InfoType.Attention:
                    MessageInfo.Style = (Style)Application.Current.Resources["AttentionIconInfoBadgeStyle"];
                    break;
                case InfoType.Informational:
                    MessageInfo.Style = (Style)Application.Current.Resources["InformationalIconInfoBadgeStyle"];
                    break;
                default:
                    break;
            }
            RectanglePointerEntered();
        }

        public void RectanglePointerEntered() => EnterStoryboard.Begin();

        public void RectanglePointerExited() => ExitStoryboard.Begin();
        #endregion

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text != null)
            {
                NavigationView_Navigate("Find", null, new object[] { sender.Text });
            }
        }

        private void AutoSuggestBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                AutoSuggestBox_QuerySubmitted(sender as AutoSuggestBox, null);
            }
        }
    }

    public enum InfoType
    {
        Success,
        Critical,
        Attention,
        Informational
    }
}
