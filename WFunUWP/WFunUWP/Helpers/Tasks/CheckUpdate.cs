using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WFunUWP.Core.Helpers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;

namespace WFunUWP.Helpers.Tasks
{
    public sealed class CheckUpdate : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            await CheckUpdateAsync(false, true);

            deferral.Complete();
        }

        public static async Task CheckUpdateAsync(bool showmassage = true, bool showtoast = false)
        {
            ResourceLoader loader = ResourceLoader.GetForViewIndependentUse();
            try
            {
                HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "wherewhere");
                string url = "https://api.github.com/repos/wherewhere/WFun-UWP/releases/latest";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject keys = JObject.Parse(responseBody);
                string[] ver = keys.Value<string>("tag_name").Replace("v", string.Empty).Split('.');
                if (ushort.Parse(ver[0]) > Package.Current.Id.Version.Major ||
                   (ushort.Parse(ver[0]) == Package.Current.Id.Version.Major && ushort.Parse(ver[1]) > Package.Current.Id.Version.Minor) ||
                   (ushort.Parse(ver[0]) == Package.Current.Id.Version.Major && ushort.Parse(ver[1]) == Package.Current.Id.Version.Minor && ushort.Parse(ver[2]) > Package.Current.Id.Version.Build))
                {
                    if (showtoast)
                    {
                        string tag = "update";
                        string group = "downloads";

                        ToastContent content = new ToastContentBuilder()
                            .AddText(loader.GetString("HasUpdateTitle"))
                            .AddText(string.Format(
                                loader.GetString("HasUpdate"),
                                Package.Current.Id.Version.Major,
                                Package.Current.Id.Version.Minor,
                                Package.Current.Id.Version.Build,
                                keys.Value<string>("tag_name")))
                            .GetToastContent();

                        ToastNotification toast = new ToastNotification(content.GetXml());

                        toast.Tag = tag;
                        toast.Group = group;

                        ToastNotificationManager.CreateToastNotifier().Show(toast);
                    }
                    else
                    {
                        string Text = string.Format(
                              loader.GetString("HasUpdate"),
                              Package.Current.Id.Version.Major,
                              Package.Current.Id.Version.Minor,
                              Package.Current.Id.Version.Build,
                              keys.Value<string>("tag_name"));
                        UIHelper.ShowMessage(Text);
                    }
                }
                else if (showmassage) { UIHelper.ShowMessage(loader.GetString("NoUpdate")); }
            }
            catch (HttpRequestException ex) { Utils.ShowHttpExceptionMessage(ex); }
        }
    }
}
