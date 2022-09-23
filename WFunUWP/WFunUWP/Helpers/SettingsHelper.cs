using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace WFunUWP.Helpers
{
    internal static partial class SettingsHelper
    {
        public const string Auth = "Auth";
        public const string TileUrl = "TileUrl";
        public const string IsFirstRun = "IsFirstRun";
        public const string UpdateDate = "UpdateDate";
        public const string IsNoPicsMode = "IsNoPicsMode";
        public const string SelectedAppTheme = "SelectedAppTheme";
        public const string ShowOtherException = "ShowOtherException";
        public const string IsDisplayOriginPicture = "IsDisplayOriginPicture";
        public const string CheckUpdateWhenLuanching = "CheckUpdateWhenLuanching";

        public static Type Get<Type>(string key) => LocalObject.Read<Type>(key);
        public static void Set<Type>(string key, Type value) => LocalObject.Save(key, value);
        public static void SetFile<Type>(string key, Type value) => LocalObject.SaveFileAsync(key, value);
        public static async Task<Type> GetFile<Type>(string key) => await LocalObject.ReadFileAsync<Type>(key);

        public static void SetDefaultSettings()
        {
            if (!LocalObject.KeyExists(Auth))
            {
                LocalObject.Save(Auth, string.Empty);
            }
            if (!LocalObject.KeyExists(TileUrl))
            {
                LocalObject.Save(TileUrl, "https://www.wpxap.com/");
            }
            if (!LocalObject.KeyExists(IsFirstRun))
            {
                LocalObject.Save(IsFirstRun, true);
            }
            if (!LocalObject.KeyExists(UpdateDate))
            {
                LocalObject.Save(UpdateDate, new DateTime());
            }
            if (!LocalObject.KeyExists(IsNoPicsMode))
            {
                LocalObject.Save(IsNoPicsMode, false);
            }
            if (!LocalObject.KeyExists(SelectedAppTheme))
            {
                LocalObject.Save(SelectedAppTheme, ElementTheme.Default);
            }
            if (!LocalObject.KeyExists(ShowOtherException))
            {
                LocalObject.Save(ShowOtherException, true);
            }
            if (!LocalObject.KeyExists(IsDisplayOriginPicture))
            {
                LocalObject.Save(IsDisplayOriginPicture, false);
            }
            if (!LocalObject.KeyExists(CheckUpdateWhenLuanching))
            {
                LocalObject.Save(CheckUpdateWhenLuanching, true);
            }
        }
    }

    internal static partial class SettingsHelper
    {
        private static readonly LocalObjectStorageHelper LocalObject = new LocalObjectStorageHelper();
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
                auth1.Value = auth2.Value = auth;
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
