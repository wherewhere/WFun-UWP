using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
