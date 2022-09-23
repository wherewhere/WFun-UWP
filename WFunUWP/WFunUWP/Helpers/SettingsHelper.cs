using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System;
using WFunUWP.Core.Helpers;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using System.Security.Cryptography;

namespace WFunUWP.Helpers
{
    internal static partial class SettingsHelper
    {
        public const string Auth = "Auth";
        public const string TileUrl = "TileUrl";
        public const string IsFirstRun = "IsFirstRun";
        public const string IsNoPicsMode = "IsNoPicsMode";
        public const string SelectedAppTheme = "SelectedAppTheme";
        public const string ShowOtherException = "ShowOtherException";
        public const string IsDisplayOriginPicture = "IsDisplayOriginPicture";
        public const string CheckUpdateWhenLuanching = "CheckUpdateWhenLuanching";

        public static Type Get<Type>(string key) => (Type)LocalSettings.Values[key];

        public static void Set(string key, object value) => LocalSettings.Values[key] = value;

        public static void SetDefaultSettings()
        {
            if (!LocalSettings.Values.ContainsKey(Auth))
            {
                LocalSettings.Values.Add(Auth, string.Empty);
            }
            if (!LocalSettings.Values.ContainsKey(TileUrl))
            {
                LocalSettings.Values.Add(TileUrl, "https://www.wpxap.com/");
            }
            if (!LocalSettings.Values.ContainsKey(IsFirstRun))
            {
                LocalSettings.Values.Add(IsFirstRun, true);
            }
            if (!LocalSettings.Values.ContainsKey(IsNoPicsMode))
            {
                LocalSettings.Values.Add(IsNoPicsMode, false);
            }
            if (!LocalSettings.Values.ContainsKey(SelectedAppTheme))
            {
                LocalSettings.Values.Add(SelectedAppTheme, (int)ElementTheme.Default);
            }
            if (!LocalSettings.Values.ContainsKey(ShowOtherException))
            {
                LocalSettings.Values.Add(ShowOtherException, true);
            }
            if (!LocalSettings.Values.ContainsKey(IsDisplayOriginPicture))
            {
                LocalSettings.Values.Add(IsDisplayOriginPicture, false);
            }
            if (!LocalSettings.Values.ContainsKey(CheckUpdateWhenLuanching))
            {
                LocalSettings.Values.Add(CheckUpdateWhenLuanching, true);
            }
        }
    }

    internal static partial class SettingsHelper
    {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;
        public static readonly MetroLog.ILogManager LogManager = MetroLog.LogManagerFactory.CreateLogManager();

        static SettingsHelper()
        {
            SetDefaultSettings();
        }

        public static async Task<bool> LoginIn(string auth)
        {
            using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
            {
                HttpCookieManager cookieManager = filter.CookieManager;
                HttpCookie auth1 = new HttpCookie("auth", "wfun.com", "/");
                HttpCookie auth2 = new HttpCookie("auth", "wpxap.com", "/");
                auth1.Value = auth2.Value=auth;
                DateTime Expires = DateTime.UtcNow.AddDays(365);
                auth1.Expires = auth2.Expires = Expires;
                cookieManager.SetCookie(auth1);
                cookieManager.SetCookie(auth2);
                return await CheckLoginInfo();
            }
        }

        public static async Task<bool> CheckLoginInfo()
        {
            try
            {
                using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
                {
                    HttpCookieManager cookieManager = filter.CookieManager;
                    string auth = string.Empty;
                    foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.BaseUri))
                    {
                        switch (item.Name)
                        {
                            case "auth":
                                auth = item.Value;
                                break;

                            default:
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(auth))
                    {
                        Logout();
                        return false;
                    }
                    else
                    {
                        Set(Auth, auth);
                        return true;
                    }
                }
            }
            catch { throw; }
        }

        public static void Logout()
        {
            using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
            {
                HttpCookieManager cookieManager = filter.CookieManager;
                foreach (HttpCookie item in cookieManager.GetCookies(UriHelper.BaseUri))
                {
                    cookieManager.DeleteCookie(item);
                }
            }
            Set(Auth, string.Empty);
        }
    }
}
