using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WFunUWP.Core.Helpers
{
    public static class NetworkHelper
    {
        public static async Task<string> GetHtmlAsync(Uri uri, bool isBackground = false)
        {
            try
            {
                HttpClient client = new HttpClient();
                return await client.GetStringAsync(uri);
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
}
