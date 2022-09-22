using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using mtuc = Microsoft.Toolkit.Uwp.Connectivity;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

namespace WFunUWP.Core.Helpers
{
    public static class RequestHelper
    {
        private static bool IsInternetAvailable => mtuc.NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable;
        private static readonly Dictionary<Uri, (DateTime date, string data)> ResponseCache = new Dictionary<Uri, (DateTime date, string data)>();
        private static readonly object locker = new object();

        internal static readonly Timer CleanCacheTimer = new Timer(o =>
        {
            if (IsInternetAvailable)
            {
                DateTime now = DateTime.Now;
                lock (locker)
                {
                    foreach (KeyValuePair<Uri, (DateTime date, string data)> i in ResponseCache)
                    {
                        if (i.Key.ToString().Contains(".com/letter") && (now - i.Value.date).TotalDays >= 2)
                        {
                            _ = ResponseCache.Remove(i.Key);
                        }
                    }
                }
            }
        }, null, TimeSpan.FromDays(2), TimeSpan.FromDays(2));

        public static async Task<(bool isSucceed, HtmlDocument result)> GetHtmlAsync(Uri uri, bool isBackground = false, bool forceRefresh = true)
        {
            string json = string.Empty;
            (bool isSucceed, HtmlDocument result) result;

            (bool isSucceed, HtmlDocument result) GetResult()
            {
                if (string.IsNullOrEmpty(json))
                {
                    Utils.ShowInAppMessage(MessageType.Message, "加载失败");
                    return (false, null);
                }
                HtmlDocument doc = new HtmlDocument();
                try { doc.LoadHtml(json); } catch { return (false, null); }
                return (true, doc);
            }

            if (forceRefresh && IsInternetAvailable)
            {
                lock (locker)
                {
                    ResponseCache.Remove(uri);
                }
            }

            bool isCached = false;

            lock (locker)
            {
                isCached = ResponseCache.ContainsKey(uri);
            }

            if (!isCached)
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetWFunCookies(uri), isBackground);
                result = GetResult();
                if (!result.isSucceed) { return result; }
                lock (locker)
                {
                    _ = ResponseCache.Remove(uri);
                    ResponseCache.Add(uri, (DateTime.Now, json));
                }
            }
            else
            {
                lock (locker)
                {
                    json = ResponseCache[uri].data;
                }
                result = GetResult();
                if (!result.isSucceed) { return result; }
            }
            return result;
        }

        public static async Task<(bool isSucceed, string result)> GetStringAsync(Uri uri, bool isBackground = false, bool forceRefresh = true)
        {
            string json = string.Empty;
            (bool isSucceed, string result) result;

            (bool isSucceed, string result) GetResult()
            {
                if (string.IsNullOrEmpty(json))
                {
                    Utils.ShowInAppMessage(MessageType.Message, "加载失败");
                    return (false, null);
                }
                else { return (true, json); }
            }

            if (forceRefresh && IsInternetAvailable)
            {
                lock (locker)
                {
                    ResponseCache.Remove(uri);
                }
            }

            bool isCached = false;

            lock (locker)
            {
                isCached = ResponseCache.ContainsKey(uri);
            }

            if (!isCached)
            {
                json = await NetworkHelper.GetSrtingAsync(uri, NetworkHelper.GetWFunCookies(uri), isBackground);
                result = GetResult();
                if (!result.isSucceed) { return result; }
                lock (locker)
                {
                    _ = ResponseCache.Remove(uri);
                    ResponseCache.Add(uri, (DateTime.Now, json));
                }
            }
            else
            {
                lock (locker)
                {
                    json = ResponseCache[uri].data;
                }
                result = GetResult();
                if (!result.isSucceed) { return result; }
            }
            return result;
        }

        public static async Task<(bool isSucceed, JToken result)> PostDataAsync(Uri uri, HttpContent content = null, bool isBackground = false)
        {
            string json = string.Empty;
            (bool isSucceed, JToken result) result;

            (bool isSucceed, JToken result) GetResult(string jsons)
            {
                if (string.IsNullOrEmpty(jsons)) { return (false, null); }
                JObject token;
                try { token = JObject.Parse(jsons); }
                catch
                {
                    Utils.ShowInAppMessage(MessageType.Message, "获取失败");
                    return (false, null);
                }
                if (token.TryGetValue("message", out JToken message))
                {
                    Utils.ShowInAppMessage(MessageType.Message, message.ToString());
                    return (false, token);
                }
                else { return (token != null && !string.IsNullOrEmpty(token.ToString()), token); }
            }

            json = await NetworkHelper.PostAsync(uri, content, NetworkHelper.GetWFunCookies(uri), isBackground);
            result = GetResult(json);

            return result;
        }

        public static async Task<(bool isSucceed, string result)> PostStringAsync(Uri uri, HttpContent content = null, bool isBackground = false)
        {
            string json = string.Empty;
            (bool isSucceed, string result) result;

            (bool isSucceed, string result) GetResult()
            {
                if (string.IsNullOrEmpty(json))
                {
                    Utils.ShowInAppMessage(MessageType.Message, "加载失败");
                    return (false, null);
                }
                else { return (true, json); }
            }

            json = await NetworkHelper.PostAsync(uri, content, NetworkHelper.GetWFunCookies(uri), isBackground);
            result = GetResult();

            return result;
        }
    }
}
