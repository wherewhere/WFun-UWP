using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WFunUWP.Pages;
using WFunUWP.Pages.FeedPages;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace WFunUWP.Helpers
{
    internal static partial class UIHelper
    {
        public const int Duration = 3000;
        public static bool IsShowingProgressBar, IsShowingMessage;
        public static bool HasTitleBar => !CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
        public static bool HasStatusBar => ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        private static readonly ObservableCollection<string> MessageList = new ObservableCollection<string>();

        private static CoreDispatcher shellDispatcher;
        public static CoreDispatcher ShellDispatcher
        {
            get => shellDispatcher;
            set
            {
                if (shellDispatcher != value)
                {
                    shellDispatcher = value;
                }
            }
        }

        public static async void ShowProgressBar()
        {
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                MainPage?.HideProgressBar();
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
                await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
            else
            {
                MainPage?.ShowProgressBar();
            }
        }

        public static async void ShowProgressBar(double value = 0)
        {
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                MainPage?.HideProgressBar();
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = value * 0.01;
                await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
            else
            {
                MainPage?.ShowProgressBar(value);
            }
        }

        public static async void PausedProgressBar()
        {
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage?.PausedProgressBar();
        }

        public static async void ErrorProgressBar()
        {
            IsShowingProgressBar = true;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage?.ErrorProgressBar();
        }

        public static async void HideProgressBar()
        {
            IsShowingProgressBar = false;
            if (HasStatusBar)
            {
                await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
            MainPage?.HideProgressBar();
        }

        public static async void ShowMessage(string message)
        {
            MessageList.Add(message);
            if (!IsShowingMessage)
            {
                IsShowingMessage = true;
                while (MessageList.Count > 0)
                {
                    if (HasStatusBar)
                    {
                        StatusBar statusBar = StatusBar.GetForCurrentView();
                        if (!string.IsNullOrEmpty(MessageList[0]))
                        {
                            statusBar.ProgressIndicator.Text = $"[{MessageList.Count}] {MessageList[0].Replace("\n", " ")}";
                            statusBar.ProgressIndicator.ProgressValue = IsShowingProgressBar ? null : (double?)0;
                            await statusBar.ProgressIndicator.ShowAsync();
                            await Task.Delay(Duration);
                        }
                        MessageList.RemoveAt(0);
                        if (MessageList.Count == 0 && !IsShowingProgressBar) { await statusBar.ProgressIndicator.HideAsync(); }
                        statusBar.ProgressIndicator.Text = string.Empty;
                    }
                    else if (MainPage != null)
                    {
                        if (!string.IsNullOrEmpty(MessageList[0]))
                        {
                            string messages = $"[{MessageList.Count}] {MessageList[0].Replace("\n", " ")}";
                            MainPage.ShowMessage(messages);
                            await Task.Delay(Duration);
                        }
                        MessageList.RemoveAt(0);
                        if (MessageList.Count == 0)
                        {
                            MainPage.ShowMessage();
                        }
                    }
                }
                IsShowingMessage = false;
            }
        }

        public static bool IsOriginSource(object source, object originalSource)
        {
            bool r = false;
            DependencyObject DependencyObject = originalSource as DependencyObject;
            if (DependencyObject.FindAscendant<Button>() == null && DependencyObject.FindAscendant<AppBarButton>() == null && originalSource.GetType() != typeof(Button) && originalSource.GetType() != typeof(AppBarButton) && originalSource.GetType() != typeof(RichEditBox))
            {
                if (source is FrameworkElement FrameworkElement)
                {
                    r = source == DependencyObject.FindAscendant(FrameworkElement.Name);
                }
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
