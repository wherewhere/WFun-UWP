using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using InAppNotify = Microsoft.Toolkit.Uwp.UI.Controls.InAppNotification;

namespace WFunUWP.Helpers
{
    internal static partial class UIHelper
    {
        public const int Duration = 3000;
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
    }
}
