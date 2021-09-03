using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace WFunUWP.Helpers
{
    internal static partial class SettingsHelper
    {
        public const string IsFirstRun = "IsFirstRun";
        public const string IsDarkMode = "IsDarkMode";
        public const string IsNoPicsMode = "IsNoPicsMode";
        public const string ShowOtherException = "ShowOtherException";
        public const string IsDisplayOriginPicture = "IsDisplayOriginPicture";
        public const string CheckUpdateWhenLuanching = "CheckUpdateWhenLuanching";
        public const string IsBackgroundColorFollowSystem = "IsBackgroundColorFollowSystem";

        public static Type Get<Type>(string key) => (Type)localSettings.Values[key];

        public static void Set(string key, object value) => localSettings.Values[key] = value;

        public static void SetDefaultSettings()
        {
            if (!localSettings.Values.ContainsKey(IsFirstRun))
            {
                localSettings.Values.Add(IsFirstRun, true);
            }
            if (!localSettings.Values.ContainsKey(IsDarkMode))
            {
                localSettings.Values.Add(IsDarkMode, false);
            }
            if (!localSettings.Values.ContainsKey(IsNoPicsMode))
            {
                localSettings.Values.Add(IsNoPicsMode, false);
            }
            if (!localSettings.Values.ContainsKey(ShowOtherException))
            {
                localSettings.Values.Add(ShowOtherException, true);
            }
            if (!localSettings.Values.ContainsKey(IsDisplayOriginPicture))
            {
                localSettings.Values.Add(IsDisplayOriginPicture, false);
            }
            if (!localSettings.Values.ContainsKey(CheckUpdateWhenLuanching))
            {
                localSettings.Values.Add(CheckUpdateWhenLuanching, true);
            }
            if (!localSettings.Values.ContainsKey(IsBackgroundColorFollowSystem))
            {
                localSettings.Values.Add(IsBackgroundColorFollowSystem, true);
            }
        }
    }

    internal enum UiSettingChangedType
    {
        LightMode,
        DarkMode,
        NoPicChanged,
    }

    internal static partial class SettingsHelper
    {
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static readonly MetroLog.ILogManager logManager = MetroLog.LogManagerFactory.CreateLogManager();
        public static readonly UISettings UISettings = new UISettings();
        public static ElementTheme Theme => Get<bool>("IsBackgroundColorFollowSystem") ? ElementTheme.Default : (Get<bool>("IsDarkMode") ? ElementTheme.Dark : ElementTheme.Light);
        public static Core.WeakEvent<UiSettingChangedType> UiSettingChanged { get; } = new Core.WeakEvent<UiSettingChangedType>();

        static SettingsHelper()
        {
            SetDefaultSettings();
            SetBackgroundTheme(UISettings, null);
            UISettings.ColorValuesChanged += SetBackgroundTheme;
            UIHelper.CheckTheme();
        }

        private static void SetBackgroundTheme(UISettings o, object _)
        {
            if (Get<bool>(IsBackgroundColorFollowSystem))
            {
                bool value = o.GetColorValue(UIColorType.Background) == Windows.UI.Colors.Black;
                Set(IsDarkMode, value);
                UiSettingChanged.Invoke(value ? UiSettingChangedType.DarkMode : UiSettingChangedType.LightMode);
            }
        }
    }
}
