using System;
using System.ComponentModel;
using WFunUWP.Helpers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WFunUWP.Pages.SettingsPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }

        private const string IssuePath = "https://github.com/wherewhere/WFun-UWP/issues";

        private bool isNoPicsMode = SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode);
        internal bool IsNoPicsMode
        {
            get => isNoPicsMode;
            set
            {
                SettingsHelper.Set(SettingsHelper.IsNoPicsMode, value);
                isNoPicsMode = SettingsHelper.Get<bool>(SettingsHelper.IsNoPicsMode);
                RaisePropertyChangedEvent();
                SettingsHelper.UISettingChanged?.Invoke(UISettingChangedType.NoPicChanged);
            }
        }

        private bool isDarkMode = SettingsHelper.Get<bool>(SettingsHelper.IsDarkMode);
        internal bool IsDarkMode
        {
            get => isDarkMode;
            set
            {
                SettingsHelper.Set(SettingsHelper.IsDarkMode, value);
                isDarkMode = SettingsHelper.Get<bool>(SettingsHelper.IsDarkMode);
                UIHelper.CheckTheme();
                RaisePropertyChangedEvent();
            }
        }

        private bool isBackgroundColorFollowSystem = SettingsHelper.Get<bool>(SettingsHelper.IsBackgroundColorFollowSystem);
        internal bool IsBackgroundColorFollowSystem
        {
            get => isBackgroundColorFollowSystem;
            set
            {
                SettingsHelper.Set(SettingsHelper.IsBackgroundColorFollowSystem, value);
                isBackgroundColorFollowSystem = SettingsHelper.Get<bool>(SettingsHelper.IsBackgroundColorFollowSystem);
                RaisePropertyChangedEvent();
                IsDarkMode = SettingsHelper.UISettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background).Equals(Windows.UI.Colors.Black);
            }
        }

        private bool showOtherException = SettingsHelper.Get<bool>(SettingsHelper.ShowOtherException);
        private bool ShowOtherException
        {
            get => showOtherException;
            set
            {
                SettingsHelper.Set(SettingsHelper.ShowOtherException, value);
                showOtherException = SettingsHelper.Get<bool>(SettingsHelper.ShowOtherException);
                RaisePropertyChangedEvent();
            }
        }

        private bool checkUpdateWhenLuanching = SettingsHelper.Get<bool>(SettingsHelper.CheckUpdateWhenLuanching);
        internal bool CheckUpdateWhenLuanching
        {
            get => checkUpdateWhenLuanching;
            set
            {
                SettingsHelper.Set(SettingsHelper.CheckUpdateWhenLuanching, value);
                checkUpdateWhenLuanching = SettingsHelper.Get<bool>(SettingsHelper.CheckUpdateWhenLuanching);
                RaisePropertyChangedEvent();
            }
        }

        private bool isCleanCacheButtonEnabled = true;
        internal bool IsCleanCacheButtonEnabled
        {
            get => isCleanCacheButtonEnabled;
            set
            {
                isCleanCacheButtonEnabled = value;
                RaisePropertyChangedEvent();
            }
        }

        private bool isCheckUpdateButtonEnabled = true;
        internal bool IsCheckUpdateButtonEnabled
        {
            get => isCheckUpdateButtonEnabled;
            set
            {
                isCheckUpdateButtonEnabled = value;
                RaisePropertyChangedEvent();
            }
        }

        internal static string VersionTextBlockText
        {
            get
            {
                string ver = $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";
                ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
                string name = loader?.GetString("AppName") ?? "智机博物馆";
                return $"{name} v{ver}";
            }
        }

        public SettingsPage() => InitializeComponent();

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
#if DEBUG
            GoToTestPage.Visibility = Visibility.Visible;
#endif
            ThemeMode.SelectedIndex = IsBackgroundColorFollowSystem ? 2 : IsDarkMode ? 1 : 0;
        }

        private void ThemeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Microsoft.UI.Xaml.Controls.RadioButtons)sender).SelectedIndex)
            {
                case 0:
                    IsBackgroundColorFollowSystem = false;
                    IsDarkMode = false;
                    break;
                case 1:
                    IsBackgroundColorFollowSystem = false;
                    IsDarkMode = true;
                    break;
                case 2:
                    IsBackgroundColorFollowSystem = true;
                    SettingsHelper.UISettingChanged?.Invoke(IsDarkMode ? UISettingChangedType.DarkMode : UISettingChangedType.LightMode);
                    break;
                default:
                    break;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as FrameworkElement).Tag as string)
            {
                case "TestPage":
                    _ = Frame.Navigate(typeof(TestPage));
                    break;
                case "FeedBack":
                    UIHelper.OpenLinkAsync(IssuePath);
                    break;
                case "LogFolder":
                    _ = await Windows.System.Launcher.LaunchFolderAsync(await ApplicationData.Current.LocalFolder.CreateFolderAsync("MetroLogs", CreationCollisionOption.OpenIfExists));
                    break;
                default:
                    break;
            }
        }

        private void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {

        }
    }
}
