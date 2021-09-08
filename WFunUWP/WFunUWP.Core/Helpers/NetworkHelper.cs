using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WFunUWP.Core.Helpers
{
    public static class NetworkHelper
    {
        public static async Task<string> GetHtmlAsync(Uri uri)
        {
            try
            {
                HttpClient client = new HttpClient();
                return await client.GetStringAsync(uri);
            }
            catch { throw; }
        }
    }
}
