using MetroLog;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using Windows.UI.Xaml;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using IObjectSerializer = Microsoft.Toolkit.Helpers.IObjectSerializer;

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
        public const string CheckUpdateWhenLaunching = "CheckUpdateWhenLaunching";

        public static Type Get<Type>(string key) => LocalObject.Read<Type>(key);
        public static void Set<Type>(string key, Type value) => LocalObject.Save(key, value);
        public static Task<Type> GetFile<Type>(string key) => LocalObject.ReadFileAsync<Type>($"Settings/{key}");
        public static Task SetFile<Type>(string key, Type value) => LocalObject.CreateFileAsync($"Settings/{key}", value);

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
            if (!LocalObject.KeyExists(CheckUpdateWhenLaunching))
            {
                LocalObject.Save(CheckUpdateWhenLaunching, true);
            }
        }
    }

    internal static partial class SettingsHelper
    {
        public static readonly ILogManager LogManager = LogManagerFactory.CreateLogManager();
        public static readonly ApplicationDataStorageHelper LocalObject = ApplicationDataStorageHelper.GetCurrent(new SystemTextJsonObjectSerializer());

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

                    if (string.IsNullOrEmpty(auth) || await RequestHelper.CheckLogin())
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

        public static bool CheckLoginInfoFast()
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

    public class SystemTextJsonObjectSerializer : IObjectSerializer
    {
        // Specify your serialization settings
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore };

        string IObjectSerializer.Serialize<T>(T value) => JsonConvert.SerializeObject(value, typeof(T), Formatting.Indented, settings);

        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, settings);
    }
}
