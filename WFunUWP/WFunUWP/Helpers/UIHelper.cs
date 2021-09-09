using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WFunUWP.Pages;
using WFunUWP.Pages.FeedPages;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using InAppNotify = Microsoft.Toolkit.Uwp.UI.Controls.InAppNotification;

namespace WFunUWP.Helpers
{
    internal static partial class UIHelper
    {
        public const int Duration = 3000;
        public static bool IsShowingProgressRing, IsShowingProgressBar, IsShowingMessage;
        public static bool HasStatusBar => ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");

        private static CoreDispatcher shellDispatcher;
        public static CoreDispatcher ShellDispatcher
        {
            get => shellDispatcher;
            set
            {
                if (shellDispatcher == null)
                {
                    shellDispatcher = value;
                }
            }
        }

        private static InAppNotify inAppNotification;
        public static InAppNotify InAppNotification
        {
            get => inAppNotification;
            set
            {
                if (inAppNotification == null)
                {
                    inAppNotification = value;
                }
            }
        }


        public static bool IsDarkTheme(ElementTheme theme)
        {
            if (theme == ElementTheme.Default)
            {
                return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            }
            return theme == ElementTheme.Dark;
        }

        public static async void CheckTheme()
        {
            while (Window.Current?.Content is null)
            {
                await Task.Delay(100);
            }

            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                foreach (CoreApplicationView item in CoreApplication.Views)
                {
                    await item.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        (Window.Current.Content as FrameworkElement).RequestedTheme = SettingsHelper.Theme;
                    });
                }

                bool IsDark = IsDarkTheme(SettingsHelper.Theme);
                Color AccentColor = (Color)Application.Current.Resources["SystemChromeMediumLowColor"];

                if (HasStatusBar)
                {
                    if (IsDark)
                    {
                        StatusBar statusBar = StatusBar.GetForCurrentView();
                        statusBar.BackgroundColor = AccentColor;
                        statusBar.ForegroundColor = Colors.White;
                        statusBar.BackgroundOpacity = 0; // 透明度
                    }
                    else
                    {
                        StatusBar statusBar = StatusBar.GetForCurrentView();
                        statusBar.BackgroundColor = AccentColor;
                        statusBar.ForegroundColor = Colors.Black;
                        statusBar.BackgroundOpacity = 0; // 透明度
                    }
                }
                else if (IsDark)
                {
                    ApplicationViewTitleBar view = ApplicationView.GetForCurrentView().TitleBar;
                    view.ButtonBackgroundColor = view.InactiveBackgroundColor = view.ButtonInactiveBackgroundColor = Colors.Transparent;
                    view.ButtonForegroundColor = Colors.White;
                }
                else
                {
                    ApplicationViewTitleBar view = ApplicationView.GetForCurrentView().TitleBar;
                    view.ButtonBackgroundColor = view.InactiveBackgroundColor = view.ButtonInactiveBackgroundColor = Colors.Transparent;
                    view.ButtonForegroundColor = Colors.Black;
                }
            }
        }

        /// <summary>
        /// 显示进度条-正常
        /// </summary>
        public static async void ShowProgressBar(uint count = 0)
        {
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
                await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
            else
            {
                MainPage?.ShowProgressBar();
            }
        }

        /// <summary>
        /// 显示进度条-暂停
        /// </summary>
        public static async void PausedProgressBar()
        {
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage?.PausedProgressBar();
        }

        /// <summary>
        /// 显示进度条-错误
        /// </summary>
        public static async void ErrorProgressBar()
        {
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage?.ErrorProgressBar();
        }

        /// <summary>
        /// 隐藏进度条
        /// </summary>
        public static async void HideProgressBar(int count = 0)
        {
            IsShowingProgressBar = false;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage?.HideProgressBar();
        }

        public static bool IsOriginSource(object source, object originalSource)
        {
            bool r = false;
            if ((originalSource as DependencyObject).FindAscendant<Button>() == null && (originalSource as DependencyObject).FindAscendant<AppBarButton>() == null && originalSource.GetType() != typeof(Button) && originalSource.GetType() != typeof(AppBarButton) && originalSource.GetType() != typeof(RichEditBox))
            {
                r = /*source == (originalSource as DependencyObject).FindAscendant<T>()*/true;
            }
            return source == originalSource || r;
        }
    }

    public enum NavigationThemeTransition
    {
        Default,
        Entrance,
        DrillIn,
        Suppress
    }

    internal static partial class UIHelper
    {
        public static MainPage MainPage;

        public static void Navigate(Type pageType, object e = null, NavigationThemeTransition Type = NavigationThemeTransition.Default)
        {
            switch (Type)
            {
                case NavigationThemeTransition.DrillIn:
                    _ = (MainPage?.NavigationViewFrame.Navigate(pageType, e, new DrillInNavigationTransitionInfo()));
                    break;
                case NavigationThemeTransition.Entrance:
                    _ = (MainPage?.NavigationViewFrame.Navigate(pageType, e, new EntranceNavigationTransitionInfo()));
                    break;
                case NavigationThemeTransition.Suppress:
                    _ = (MainPage?.NavigationViewFrame.Navigate(pageType, e, new SuppressNavigationTransitionInfo()));
                    break;
                case NavigationThemeTransition.Default:
                    _ = (MainPage?.NavigationViewFrame.Navigate(pageType, e));
                    break;
                default:
                    _ = (MainPage?.NavigationViewFrame.Navigate(pageType, e));
                    break;
            }
        }

        private static readonly ImmutableArray<string> routes = new string[]
        {
            "/u/",
            "/tag/",
            "/forum",
            "/thread",
            "http:",
            "https:",
            "www.wpxap.com",
        }.ToImmutableArray();

        private static bool IsFirst(this string str, int i) => str.IndexOf(routes[i], StringComparison.Ordinal) == 0;

        private static string Replace(this string str, int oldText)
        {
            return oldText == -1
                ? str.Replace("http://www.wpxap.com", string.Empty)
                : oldText == -2
                    ? str.Replace("https://www.wpxap.com", string.Empty)
                    : oldText == -3
                                    ? str.Replace("www.wpxap.com", string.Empty)
                                    : oldText < 0 ? throw new Exception($"i = {oldText}") : str.Replace(routes[oldText], string.Empty);
        }

        public static void OpenLinkAsync(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) { return; }
            int i = 0;
            if (str.IsFirst(i++))
            {
                string u = str.Replace(i - 1);
                Navigate(typeof(FeedListPage), new object[] { u, FeedListType.User });
            }
            else if (str.IsFirst(i++))
            {
                string u = str.Replace(i - 1);
                Navigate(typeof(FeedListPage), new object[] { u, FeedListType.Tag });
            }
            else if (str.IsFirst(i++))
            {
                string u = str.Replace(i - 1);
                Navigate(typeof(FeedListPage), new object[] { new Regex(@".*?-(\d+)-[\d+].html").Match(u).Groups[1].Value, FeedListType.Forum });
            }
            else if (str.IsFirst(i++))
            {
                string u = str.Replace(i - 1);
                Navigate(typeof(FeedShellPage), new object[] { new Regex(@".*?-(\d+)-[\d+]-[\d+].html").Match(u).Groups[1].Value });
            }
            else if (str.IsFirst(i++))
            {
                if (str.Contains("http://www.wpxap.com"))
                {
                    OpenLinkAsync(str.Replace(-1));
                }
                else
                {
                    _ = Launcher.LaunchUriAsync(new Uri(str));
                }
            }
            else if (str.IsFirst(i++))
            {
                if (str.Contains("https://www.wpxap.com"))
                {
                    OpenLinkAsync(str.Replace(-2));
                }
                else
                {
                    _ = Launcher.LaunchUriAsync(new Uri(str));
                }
            }
            else if (str.IsFirst(i++))
            {
                OpenLinkAsync(str.Replace(-3));
            }
        }
    }
}
