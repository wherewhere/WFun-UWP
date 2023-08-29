using System;

namespace WFunUWP.Core.Helpers
{
    /// <summary> 程序支持的能从服务器中获取的数据的类型。 </summary>
    public enum UriType
    {
        Login,
        GetNewsFeeds,
        GetTagDetail,
        GetUserDetail,
        GetFeedDetail,
        GetForumDetail,
    }

    public static class UriHelper
    {
        public static readonly Uri BaseUri = new Uri("https://www.wpxap.com");

        public static Uri GetUri(UriType type, params object[] args)
        {
            string u = string.Format(GetTemplate(type), args);
            return new Uri(BaseUri, u);
        }

        private static string GetTemplate(UriType type)
        {
            switch (type)
            {
                case UriType.Login: return "/api/v1/login";
                case UriType.GetNewsFeeds: return "/new/{0}.html";
                case UriType.GetTagDetail: return "/tag/{0}";
                case UriType.GetUserDetail: return "/u/{0}";
                case UriType.GetFeedDetail: return "/thread-{0}-1-1.html";
                case UriType.GetForumDetail: return "/forum-{0}-{1}.html";
                default: throw new ArgumentException($"{typeof(UriType).FullName}值错误");
            }
        }
    }
}
