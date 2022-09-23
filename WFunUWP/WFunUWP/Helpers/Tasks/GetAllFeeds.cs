using LiteDB;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WFunUWP.Models;
using WFunUWP.Pages.FeedPages;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace WFunUWP.Helpers.Tasks
{
    internal class GetAllFeeds : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            await GetAndSaveFeeds();
            deferral.Complete();
        }

        public static async Task GetAndSaveFeeds()
        {
            NewsDS NewsDS = new NewsDS();
            string FeedPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Feeds.db");
            while (NewsDS.HasMoreItems)
            {
                _ = await NewsDS.LoadMoreItemsAsync(20);
                await Task.Delay(200);
            }
            using (LiteDatabase db = new LiteDatabase(FeedPath))
            {
                ILiteCollection<FeedListModel> FeedLists = db.GetCollection<FeedListModel>();
                foreach (object items in NewsDS)
                {
                    if (items is FeedListModel Feed)
                    {
                        _ = FeedLists.Upsert(new Regex(@".*?-(\d+)-[\d+]-[\d+].html").Match(Feed.Url).Groups[1].Value, Feed);
                    }
                }
            }
        }
    }
}
