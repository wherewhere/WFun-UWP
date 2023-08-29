using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;

namespace WFunUWP.Core.Helpers
{
    public static partial class NetworkHelper
    {
        public static IEnumerable<(string name, string value)> GetWFunCookies(Uri uri)
        {
            using (HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter())
            {
                Windows.Web.Http.HttpCookieManager cookieManager = filter.CookieManager;
                foreach (Windows.Web.Http.HttpCookie item in cookieManager.GetCookies(GetHost(uri)))
                {
                    if (item.Name == "auth" ||
                        item.Name == "Hm_lvt_23b5079e4921136acc177490b1f1f4fa" ||
                        item.Name == "Hm_lpvt_23b5079e4921136acc177490b1f1f4fa")
                    {
                        yield return (item.Name, item.Value);
                    }
                }
            }
        }

        private static void ReplaceCoolapkCookie(this CookieContainer container, IEnumerable<(string name, string value)> cookies, Uri uri)
        {
            if (cookies == null) { return; }

            foreach ((string name, string value) in cookies)
            {
                container.SetCookies(uri.GetHost(), $"{name}={value}");
            }
        }

        public static void BeforeGetOrPost(this HttpClientHandler clientHandler, IEnumerable<(string name, string value)> coolapkCookies, Uri uri)
        {
            clientHandler.CookieContainer.ReplaceCoolapkCookie(coolapkCookies, uri);
        }
    }

    public static partial class NetworkHelper
    {
        public static async Task<string> PostAsync(Uri uri, HttpContent content, IEnumerable<(string name, string value)> coolapkCookies, bool isBackground)
        {
            try
            {
                const string name = "X-Requested-With";
                HttpResponseMessage response;
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(name, "XMLHttpRequest");
                    response = await client.PostAsync(uri, content);
                    client.DefaultRequestHeaders.Remove(name);
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                if (!isBackground) { Utils.ShowHttpExceptionMessage(e); }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Stream> GetStreamAsync(Uri uri, IEnumerable<(string name, string value)> coolapkCookies, bool isBackground = false)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.BeforeGetOrPost(coolapkCookies, uri);
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    return await client.GetStreamAsync(uri);
                }
            }
            catch (HttpRequestException e)
            {
                if (!isBackground) { Utils.ShowHttpExceptionMessage(e); }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<string> GetSrtingAsync(Uri uri, IEnumerable<(string name, string value)> coolapkCookies, bool isBackground = false)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.BeforeGetOrPost(coolapkCookies, uri);
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    return await client.GetStringAsync(uri);
                }
            }
            catch (HttpRequestException e)
            {
                if (!isBackground) { Utils.ShowHttpExceptionMessage(e); }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public static partial class NetworkHelper
    {
        public static Uri GetHost(this Uri uri)
        {
            return new Uri("https://" + uri.Host);
        }

        public static string ExpandShortUrl(this Uri ShortUrl)
        {
            string NativeUrl = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(ShortUrl);
            try { _ = req.HaveResponse; }
            catch (WebException ex)
            {
                HttpWebResponse res = ex.Response as HttpWebResponse;
                if (res.StatusCode == HttpStatusCode.Found)
                { NativeUrl = res.Headers["Location"]; }
            }
            return NativeUrl ?? ShortUrl.ToString();
        }

        public static Uri ValidateAndGetUri(this string uriString)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(uriString);
            }
            catch (FormatException)
            {
            }
            return uri;
        }
    }
}
